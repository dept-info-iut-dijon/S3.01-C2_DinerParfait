using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des allergènes.
    /// </summary>
    public class AllergeneDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de AllergeneDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public AllergeneDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère tous les allergènes de la base de données.
        /// </summary>
        /// <returns>Liste de tous les allergènes.</returns>
        public List<Allergene> GetAll()
        {
            List<Allergene> allergenes = new List<Allergene>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Nom, Description FROM Allergenes";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allergenes.Add(new Allergene
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                        });
                    }
                }
            }
            return allergenes;
        }

        /// <summary>
        /// Récupère les allergènes associés à un client spécifique.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des allergènes du client.</returns>
        public List<Allergene> GetAllergenesByClient(int clientId)
        {
            List<Allergene> allergenes = new List<Allergene>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT a.Id, a.Nom, a.Description 
                                FROM Allergenes a
                                INNER JOIN ClientAllergene ca ON a.Id = ca.AllergeneId
                                WHERE ca.ClientId = @ClientId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allergenes.Add(new Allergene
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return allergenes;
        }

        /// <summary>
        /// Associe une liste d'allergènes à un client.
        /// Supprime d'abord les anciennes associations puis ajoute les nouvelles.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes à associer.</param>
        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                
                string deleteQuery = "DELETE FROM ClientAllergene WHERE ClientId = @ClientId";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }

                foreach (var allergeneId in allergeneIds)
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
        /// Ajoute un nouvel allergène à la base de données.
        /// </summary>
        /// <param name="allergene">L'allergène à ajouter.</param>
        public void AjouterAllergene(Allergene allergene)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Allergenes (Nom, Description) VALUES (@Nom, @Description)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", allergene.Nom);
                    command.Parameters.AddWithValue("@Description", allergene.Description);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
