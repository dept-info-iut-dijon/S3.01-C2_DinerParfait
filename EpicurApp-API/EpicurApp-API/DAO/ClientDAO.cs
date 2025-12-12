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
                    (Nom, Prenom, Email, Telephone, platsNonApprecies, preferences, RestaurantId)
                    VALUES (@Nom, @Prenom, @Email, @Telephone, @PlatsNonApprecies, @Preferences, @RestaurantId);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email ?? "");
                    command.Parameters.AddWithValue("@Telephone", client.Telephone ?? "");
                    command.Parameters.AddWithValue("@PlatsNonApprecies", ConvertirPlatsEnIds(client.PlatsNonApprecies));
                    command.Parameters.AddWithValue("@Preferences", client.Preferences ?? "");
                    command.Parameters.AddWithValue("@RestaurantId", client.RestaurantId);

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
        public Client? GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences, RestaurantId
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
                            Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            RestaurantId = reader.GetInt32(7)
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

                    string query = "SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences, RestaurantId FROM Clients ORDER BY Nom, Prenom";

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
                                Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                RestaurantId = reader.GetInt32(7)
                            };

                            clients.Add(client);
                        }
                    }
                    // Charger les allergènes pour chaque client
                    foreach (Client client in clients)
                    {
                        string allergeneQuery = @"SELECT a.Id, a.Nom, a.Description
                                         FROM Allergenes a
                                         INNER JOIN ClientAllergene ca ON a.Id = ca.AllergeneId
                                         WHERE ca.ClientId = @ClientId";

                        using (SqliteCommand cmd = new SqliteCommand(allergeneQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@ClientId", client.Id);
                            using (SqliteDataReader allergeneReader = cmd.ExecuteReader())
                            {
                                while (allergeneReader.Read())
                                {
                                    Allergene allergene = new Allergene
                                    {
                                        Id = allergeneReader.GetInt32(0),
                                        Nom = allergeneReader.GetString(1),
                                        Description = allergeneReader.IsDBNull(2) ? "" : allergeneReader.GetString(2)
                                    };
                                    client.Allergenes.Add(allergene);
                                }
                            }
                        }
                    }
                }

                return clients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERREUR: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Récupère tous les clients d'un restaurant spécifique.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des clients du restaurant.</returns>
        public List<Client> GetAllByRestaurantId(int restaurantId)
        {
            List<Client> clients = new List<Client>();

            try
            {
                using (SqliteConnection connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences, RestaurantId FROM Clients WHERE RestaurantId = @RestaurantId ORDER BY Nom, Prenom";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RestaurantId", restaurantId);

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
                                    Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    RestaurantId = reader.GetInt32(7)
                                };

                                clients.Add(client);
                            }
                        }
                    }

                    // Charger les allergènes pour chaque client
                    foreach (Client client in clients)
                    {
                        string allergeneQuery = @"SELECT a.Id, a.Nom, a.Description
                                         FROM Allergenes a
                                         INNER JOIN ClientAllergene ca ON a.Id = ca.AllergeneId
                                         WHERE ca.ClientId = @ClientId";

                        using (SqliteCommand cmd = new SqliteCommand(allergeneQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@ClientId", client.Id);
                            using (SqliteDataReader allergeneReader = cmd.ExecuteReader())
                            {
                                while (allergeneReader.Read())
                                {
                                    Allergene allergene = new Allergene
                                    {
                                        Id = allergeneReader.GetInt32(0),
                                        Nom = allergeneReader.GetString(1),
                                        Description = allergeneReader.IsDBNull(2) ? "" : allergeneReader.GetString(2)
                                    };
                                    client.Allergenes.Add(allergene);
                                }
                            }
                        }
                    }
                }

                return clients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERREUR: {ex.Message}");
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

                //Supprimer les liens avec les allergenes
                string deleteAllergies = "DELETE FROM ClientAllergene WHERE ClientId = @Id";
                using (var cmd1 = new SqliteCommand(deleteAllergies, connection))
                {
                    cmd1.Parameters.AddWithValue("@Id", id);
                    cmd1.ExecuteNonQuery();
                }

                //Supprimer les liens avec les menus
                string deleteMenus = "DELETE FROM ClientMenu WHERE ClientId = @Id";
                using (var cmd2 = new SqliteCommand(deleteMenus, connection))
                {
                    cmd2.Parameters.AddWithValue("@Id", id);
                    cmd2.ExecuteNonQuery();
                }

                //Supprimer le client
                string deleteClient = "DELETE FROM Clients WHERE Id = @Id;";
                using (var cmd3 = new SqliteCommand(deleteClient, connection))
                {
                    cmd3.Parameters.AddWithValue("@Id", id);
                    cmd3.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Recherche un client par son identifiant avec ses allergènes associés.
        /// </summary>
        /// <param name="id">Identifiant du client.</param>
        /// <returns>Le client avec ses allergènes ou null.</returns>
        public Client? RechercherClientParId(int id)
        {
            Client? client = GetById(id); // charge les infos de base

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
        public async Task<Client?> GetByIdWithHistoryAsync(int id)
        {
            Client? client = null;

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Charger le client
                const string clientQuery = @"SELECT Id, Nom, Prenom, Email, Telephone, PlatsNonApprecies, Preferences, RestaurantId
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
                                Preferences = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                RestaurantId = reader.GetInt32(7)
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

        /// <summary>
        /// Récupère les clients réguliers (3 visites ou plus sur l'année).
        /// </summary>
        /// <returns>Liste des clients réguliers.</returns>
        /// <summary>
        /// Récupère les clients réguliers (3 visites ou plus sur l'année).
        /// </summary>
        /// <param name="restaurantId">Filtre par restaurant (optionnel)</param>
        /// <returns>Liste des clients réguliers.</returns>
        public List<Client> GetClientsReguliers(int? restaurantId = null)
        {
            List<Client> clientsReguliers = new List<Client>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT c.Id, c.Nom, c.Prenom, c.Email, c.Telephone, c.PlatsNonApprecies, c.Preferences, COUNT(r.Id) as NbVisites
            FROM Clients c
            INNER JOIN Repas r ON c.Id = r.ClientId
            WHERE r.Date >= date('now', '-1 year')";

                if (restaurantId.HasValue)
                {
                    query += " AND c.RestaurantId = @RestaurantId";
                }

                query += @"
            GROUP BY c.Id
            HAVING COUNT(r.Id) >= 3
            ORDER BY NbVisites DESC";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    if (restaurantId.HasValue)
                    {
                        command.Parameters.AddWithValue("@RestaurantId", restaurantId.Value);
                    }

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

                            clientsReguliers.Add(client);
                        }
                    }
                }
            }

            return clientsReguliers;
        }

        /// <summary>
        /// Récupère les clients inactifs (pas de visite depuis plus de 60 jours).
        /// </summary>
        /// <returns>Liste des clients inactifs.</returns>
        /// <summary>
        /// Récupère les clients inactifs (pas de visite depuis plus de 60 jours).
        /// </summary>
        /// <param name="restaurantId">Filtre par restaurant (optionnel)</param>
        /// <returns>Liste des clients inactifs.</returns>
        public List<Client> GetClientsInactifs(int? restaurantId = null)
        {
            List<Client> clientsInactifs = new List<Client>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT c.Id, c.Nom, c.Prenom, c.Email, c.Telephone, c.PlatsNonApprecies, c.Preferences
            FROM Clients c
            WHERE c.Id NOT IN (
                SELECT DISTINCT r.ClientId
                FROM Repas r
                WHERE r.Date >= date('now', '-60 days')
            )";

                if (restaurantId.HasValue)
                {
                    query += " AND c.RestaurantId = @RestaurantId";
                }

                query += " ORDER BY c.Nom, c.Prenom";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    if (restaurantId.HasValue)
                    {
                        command.Parameters.AddWithValue("@RestaurantId", restaurantId.Value);
                    }

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

                            clientsInactifs.Add(client);
                        }
                    }
                }
            }

            return clientsInactifs;
        }

        /// <summary>
        /// Récupère les clients VIP (7+ visites sur l'année écoulée)
        /// </summary>
        /// <summary>
        /// Récupère les clients VIP (7+ visites sur l'année écoulée)
        /// </summary>
        /// <param name="restaurantId">Filtre par restaurant (optionnel)</param>
        public List<Client> GetClientsVIP(int? restaurantId = null)
        {
            List<Client> clientsVIP = new List<Client>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
            SELECT c.Id, c.Nom, c.Prenom, c.Email, c.Telephone, c.PlatsNonApprecies, c.Preferences, COUNT(r.Id) as NbVisites
            FROM Clients c
            INNER JOIN Repas r ON c.Id = r.ClientId
            WHERE r.Date >= date('now', '-1 year')";

                if (restaurantId.HasValue)
                {
                    query += " AND c.RestaurantId = @RestaurantId";
                }

                query += @"
            GROUP BY c.Id
            HAVING COUNT(r.Id) >= 7
            ORDER BY NbVisites DESC";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    if (restaurantId.HasValue)
                    {
                        command.Parameters.AddWithValue("@RestaurantId", restaurantId.Value);
                    }

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

                            clientsVIP.Add(client);
                        }
                    }
                }
            }

            return clientsVIP;
        }
    }
}