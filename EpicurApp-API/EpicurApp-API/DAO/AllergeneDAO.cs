using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des allergènes dans la base de données
    /// </summary>
    public class AllergeneDAO : IAllergeneDAO
    {
        private readonly DatabaseConfiguration _dbConfig;

        public AllergeneDAO(DatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public List<Allergene> GetAll()
        {
            List<Allergene> allergenes = new List<Allergene>();

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();
                string query = "SELECT Id, Nom, Description FROM Allergenes";

                using (SqliteCommand command = new SqliteCommand(query, connection))
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

                        allergenes.Add(allergene);
                    }
                }
            }
            return allergenes;
        }

        public List<Allergene> GetAllergenesByClient(int clientId)
        {
            List<Allergene> allergenes = new List<Allergene>();

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string query = @"SELECT a.Id, a.Nom, a.Description 
                                 FROM Allergenes a
                                 INNER JOIN ClientAllergene ca ON a.Id = ca.AllergeneId
                                 WHERE ca.ClientId = @ClientId";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

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

                            allergenes.Add(allergene);
                        }
                    }
                }
            }

            return allergenes;
        }

        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string deleteQuery = "DELETE FROM ClientAllergene WHERE ClientId = @ClientId";

                using (SqliteCommand deleteCommand = new SqliteCommand(deleteQuery, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@ClientId", clientId);
                    deleteCommand.ExecuteNonQuery();
                }

                foreach (int allergeneId in allergeneIds)
                {
                    string insertQuery = "INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (@ClientId, @AllergeneId)";

                    using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@ClientId", clientId);
                        insertCommand.Parameters.AddWithValue("@AllergeneId", allergeneId);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AjouterAllergene(Allergene allergene)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string query = "INSERT INTO Allergenes (Nom, Description) VALUES (@Nom, @Description)";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", allergene.Nom);
                    command.Parameters.AddWithValue("@Description", allergene.Description);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
