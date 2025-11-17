using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des clients dans la base de données
    /// </summary>
    public class ClientDAO : IClientDAO
    {
        private readonly DatabaseConfiguration _dbConfig;
        private readonly PlatDAO _platDAO;

        public ClientDAO(DatabaseConfiguration dbConfig, PlatDAO platDAO)
        {
            _dbConfig = dbConfig;
            _platDAO = platDAO;
        }

        public void AjouterClient(Client client)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string query = @"INSERT INTO Clients
                    (Nom, Prenom, Email, Telephone, Preferences)
                    VALUES (@Nom, @Prenom, @Email, @Telephone, @Preferences);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email ?? "");
                    command.Parameters.AddWithValue("@Telephone", client.Telephone ?? "");
                    command.Parameters.AddWithValue("@Preferences", client.Preferences ?? "");

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        client.Id = Convert.ToInt32(result);
                    }
                }

                if (client.PlatsNonApprecies != null && client.PlatsNonApprecies.Count > 0)
                {
                    AjouterPlatsNonAppreciesAuClient(
                        client.Id,
                        client.PlatsNonApprecies.Select(p => p.Id).ToList()
                    );
                }
            }
        }

        public Client GetById(int id)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Prenom, Email, Telephone, Preferences
                    FROM Clients WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        Client client = new Client
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Prenom = reader.GetString(2),
                            Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Telephone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Preferences = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        };

                        client.PlatsNonApprecies = GetPlatsNonAppreciesByClientId(id);

                        return client;
                    }
                }
            }
        }

        public List<Client> GetAll()
        {
            List<Client> clients = new List<Client>();

            try
            {
                using (SqliteConnection connection = _dbConfig.CreateConnection())
                {
                    connection.Open();

                    string query = "SELECT Id, Nom, Prenom, Email, Telephone, Preferences FROM Clients ORDER BY Nom, Prenom";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientId = reader.GetInt32(0);

                            Client client = new Client
                            {
                                Id = clientId,
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Telephone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Preferences = reader.IsDBNull(5) ? "" : reader.GetString(5)
                            };

                            clients.Add(client);
                        }
                    }
                }

                return clients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERREUR: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public void ModifierClient(Client client)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string query = @"UPDATE Clients SET
                    Nom = @Nom,
                    Prenom = @Prenom,
                    Email = @Email,
                    Telephone = @Telephone,
                    Preferences = @Preferences
                    WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", client.Id);
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email ?? "");
                    command.Parameters.AddWithValue("@Telephone", client.Telephone ?? "");
                    command.Parameters.AddWithValue("@Preferences", client.Preferences ?? "");

                    command.ExecuteNonQuery();
                }

                if (client.PlatsNonApprecies != null)
                {
                    AjouterPlatsNonAppreciesAuClient(
                        client.Id,
                        client.PlatsNonApprecies.Select(p => p.Id).ToList()
                    );
                }
            }
        }

        public void SupprimerClient(int id)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string query = "DELETE FROM Clients WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Client RechercherClientParId(int id)
        {
            Client client = GetById(id);

            if (client != null)
            {
                using (SqliteConnection connection = _dbConfig.CreateConnection())
                {
                    connection.Open();

                    string query = @"SELECT a.Id, a.Nom, a.Description
                             FROM Allergenes a
                             INNER JOIN ClientAllergene ca ON a.Id = ca.AllergeneId
                             WHERE ca.ClientId = @ClientId";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", id);

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Allergene allergene = new Allergene
                                {
                                    Id = reader.GetInt32(0),
                                    Nom = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                                };

                                client.Allergenes.Add(allergene);
                            }
                        }
                    }
                }
            }

            return client;
        }

        public async Task<Client> GetByIdWithHistoryAsync(int id)
        {
            Client client = null;

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                await connection.OpenAsync();

                const string clientQuery = @"SELECT Id, Nom, Prenom, Email, Telephone, Preferences
                    FROM Clients WHERE Id=@Id";

                using (SqliteCommand cmdClient = new SqliteCommand(clientQuery, connection))
                {
                    cmdClient.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = await cmdClient.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            client = new Client
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Telephone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Preferences = reader.IsDBNull(5) ? "" : reader.GetString(5)
                            };
                        }
                    }
                }

                if (client == null) return null;

                client.PlatsNonApprecies = GetPlatsNonAppreciesByClientId(id);

                const string allergenesQuery = @"SELECT a.Id, a.Nom, a.Description 
                    FROM Allergenes a 
                    JOIN ClientAllergene ca ON a.Id = ca.AllergeneId 
                    WHERE ca.ClientId = @ClientId";

                using (SqliteCommand cmdAllergenes = new SqliteCommand(allergenesQuery, connection))
                {
                    cmdAllergenes.Parameters.AddWithValue("@ClientId", id);

                    using (SqliteDataReader reader = await cmdAllergenes.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            client.Allergenes.Add(new Allergene
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                            });
                        }
                    }
                }

                const string historyQuery = @"SELECT m.Id, m.Nom, m.Date, m.Statut 
                    FROM Menus m 
                    JOIN ClientMenu cm ON m.Id = cm.MenuId 
                    WHERE cm.ClientId = @ClientId 
                    ORDER BY m.Date DESC";

                using (SqliteCommand cmdHistory = new SqliteCommand(historyQuery, connection))
                {
                    cmdHistory.Parameters.AddWithValue("@ClientId", id);

                    using (SqliteDataReader reader = await cmdHistory.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            client.HistoriqueRepas.Add(new Menu
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Date = DateTime.Parse(reader.GetString(2)),
                                Statut = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return client;
        }

        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string deleteQuery = "DELETE FROM ClientAllergene WHERE ClientId = @ClientId";

                using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }

                foreach (int allergeneId in allergeneIds)
                {
                    string insertQuery = "INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (@ClientId, @AllergeneId)";

                    using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", clientId);
                        command.Parameters.AddWithValue("@AllergeneId", allergeneId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private List<Plat> GetPlatsNonAppreciesByClientId(int clientId)
        {
            List<Plat> plats = new List<Plat>();

            const string query = @"SELECT p.Id
                                  FROM Plats p
                                  INNER JOIN ClientPlat cp ON p.Id = cp.PlatId
                                  WHERE cp.ClientId = @ClientId";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int platId = reader.GetInt32(0);
                            Plat plat = _platDAO.GetById(platId);

                            if (plat != null)
                            {
                                plats.Add(plat);
                            }
                        }
                    }
                }
            }

            return plats;
        }

        private void AjouterPlatsNonAppreciesAuClient(int clientId, List<int> platIds)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string deleteQuery = "DELETE FROM ClientPlat WHERE ClientId = @ClientId";

                using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }

                foreach (int platId in platIds)
                {
                    string insertQuery = "INSERT INTO ClientPlat (ClientId, PlatId) VALUES (@ClientId, @PlatId)";

                    using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", clientId);
                        command.Parameters.AddWithValue("@PlatId", platId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
