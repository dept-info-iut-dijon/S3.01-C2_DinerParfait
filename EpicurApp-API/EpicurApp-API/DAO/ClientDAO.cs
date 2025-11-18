using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des clients.
    /// </summary>
    public class ClientDAO : IClientDAO
    {
        private readonly string _connectionString;
        private readonly IPlatDAO _platDAO;

        /// <summary>
        /// Initialise une nouvelle instance de ClientDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        /// <param name="platDAO">DAO pour accéder aux plats.</param>
        public ClientDAO(DatabaseConfiguration databaseConfiguration, IPlatDAO platDAO)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
            _platDAO = platDAO;
        }

        /// <summary>
        /// Ajoute un nouveau client dans la base de données.
        /// </summary>
        /// <param name="client">Le client à ajouter.</param>
        public void AjouterClient(Client client)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Clients 
                    (Nom, Prenom, Email, Telephone, platsNonApprecies, preferences) 
                    VALUES (@Nom, @Prenom, @Email, @Telephone, @PlatsNonApprecies, @Preferences);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email ?? "");
                    command.Parameters.AddWithValue("@Telephone", client.Telephone ?? "");
                    command.Parameters.AddWithValue("@PlatsNonApprecies", ConvertirPlatsEnIds(client.PlatsNonApprecies));
                    command.Parameters.AddWithValue("@Preferences", client.Preferences ?? "");

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        client.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        /// <summary>
        /// Récupère un client par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du client.</param>
        /// <returns>Le client trouvé ou null.</returns>
        public Client GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences 
                    FROM Clients WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        string platsNonAppreciesIds = reader.IsDBNull(5) ? "" : reader.GetString(5);

                        var client = new Client
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Prenom = reader.GetString(2),
                            Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Telephone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            PlatsNonApprecies = ConvertirIdsEnPlats(platsNonAppreciesIds),
                            Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6)
                        };

                        return client;
                    }
                }
            }
        }

        /// <summary>
        /// Récupère tous les clients de la base de données.
        /// </summary>
        /// <returns>Liste de tous les clients.</returns>
        public List<Client> GetAll()
        {
            List<Client> clients = new List<Client>();

            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine(" Connexion ouverte");

                    string query = "SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences FROM Clients ORDER BY Nom, Prenom";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string platsNonAppreciesIds = reader.IsDBNull(5) ? "" : reader.GetString(5);

                            Client client = new Client
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Telephone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                PlatsNonApprecies = ConvertirIdsEnPlats(platsNonAppreciesIds),
                                Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6)
                            };

                            clients.Add(client);
                        }
                    }

                    Console.WriteLine($" {clients.Count} clients récupérés");

                
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

        /// <summary>
        /// Met à jour les informations d'un client existant.
        /// </summary>
        /// <param name="client">Le client avec les informations mises à jour.</param>
        public void ModifierClient(Client client)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"UPDATE Clients SET 
                    Nom = @Nom, 
                    Prenom = @Prenom, 
                    Email = @Email, 
                    Telephone = @Telephone, 
                    PlatsNonApprecies = @PlatsNonApprecies, 
                    Preferences = @Preferences 
                    WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", client.Id);
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email ?? "");
                    command.Parameters.AddWithValue("@Telephone", client.Telephone ?? "");
                    command.Parameters.AddWithValue("@PlatsNonApprecies", ConvertirPlatsEnIds(client.PlatsNonApprecies));
                    command.Parameters.AddWithValue("@Preferences", client.Preferences ?? "");

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Supprime un client de la base de données.
        /// </summary>
        /// <param name="id">Identifiant du client à supprimer.</param>
        public void SupprimerClient(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Clients WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Recherche un client par son identifiant avec ses allergènes associés.
        /// </summary>
        /// <param name="id">Identifiant du client.</param>
        /// <returns>Le client avec ses allergènes ou null.</returns>
        public Client RechercherClientParId(int id)
        {
            Client client = GetById(id); // charge les infos de base

            if (client != null)
            {
                // charger les allergènes associés
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
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


        /// <summary>
        /// Récupère un client par son identifiant avec son historique de repas de manière asynchrone.
        /// </summary>
        /// <param name="id">Identifiant du client.</param>
        /// <returns>Le client avec son historique ou null.</returns>
        public async Task<Client> GetByIdWithHistoryAsync(int id)
        {
            Client client = null;

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Charger le client
                const string clientQuery = @"SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences 
                    FROM Clients WHERE Id=@Id";

                using (var cmdClient = new SqliteCommand(clientQuery, connection))
                {
                    cmdClient.Parameters.AddWithValue("@Id", id);
                    using (var reader = await cmdClient.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string platsNonAppreciesIds = reader.IsDBNull(5) ? "" : reader.GetString(5);

                            client = new Client
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Telephone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                PlatsNonApprecies = ConvertirIdsEnPlats(platsNonAppreciesIds),
                                Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6)
                            };
                        }
                    }
                }

                if (client == null) return null;

                // Charger les allergènes du client
                const string allergenesQuery = @"SELECT a.Id, a.Nom, a.Description 
                    FROM Allergenes a 
                    JOIN ClientAllergene ca ON a.Id = ca.AllergeneId 
                    WHERE ca.ClientId = @ClientId";

                using (var cmdAllergenes = new SqliteCommand(allergenesQuery, connection))
                {
                    cmdAllergenes.Parameters.AddWithValue("@ClientId", id);
                    using (var reader = await cmdAllergenes.ExecuteReaderAsync())
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

                // Charger l'historique des menus
                const string historyQuery = @"SELECT m.Id, m.Nom, m.Date, m.Statut 
                    FROM Menus m 
                    JOIN ClientMenu cm ON m.Id = cm.MenuId 
                    WHERE cm.ClientId = @ClientId 
                    ORDER BY m.Date DESC";

                using (var cmdHistory = new SqliteCommand(historyQuery, connection))
                {
                    cmdHistory.Parameters.AddWithValue("@ClientId", id);
                    using (var reader = await cmdHistory.ExecuteReaderAsync())
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

        /// <summary>
        /// Associe une liste d'allergènes à un client.
        /// Supprime d'abord les anciennes associations puis ajoute les nouvelles.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes.</param>
        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Supprimer les anciennes associations
                string deleteQuery = "DELETE FROM ClientAllergene WHERE ClientId = @ClientId";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }

                // Ajouter les nouvelles associations
                foreach (int allergeneId in allergeneIds)
                {
                    string insertQuery = "INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (@ClientId, @AllergeneId)";
                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", clientId);
                        command.Parameters.AddWithValue("@AllergeneId", allergeneId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Convertit une liste de plats en une chaîne d'IDs séparés par des virgules.
        /// </summary>
        /// <param name="plats">Liste de plats à convertir.</param>
        /// <returns>Chaîne d'IDs séparés par des virgules.</returns>
        private string ConvertirPlatsEnIds(List<Plat> plats)
        {
            if (plats == null || plats.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(",", plats.Select(p => p.Id));
        }

        /// <summary>
        /// Convertit une chaîne d'IDs séparés par des virgules en une liste de plats.
        /// </summary>
        /// <param name="ids">Chaîne d'IDs séparés par des virgules.</param>
        /// <returns>Liste de plats correspondants.</returns>
        private List<Plat> ConvertirIdsEnPlats(string ids)
        {
            List<Plat> plats = new List<Plat>();

            if (string.IsNullOrWhiteSpace(ids))
            {
                return plats;
            }

            string[] idsArray = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string idString in idsArray)
            {
                if (int.TryParse(idString.Trim(), out int platId))
                {
                    Plat? plat = _platDAO.GetById(platId);
                    if (plat != null)
                    {
                        plats.Add(plat);
                    }
                }
            }

            return plats;
        }
    }
}