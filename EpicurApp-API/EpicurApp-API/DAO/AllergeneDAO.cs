using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace EpicurApp_API.DAO
{
    public class AllergeneDAO
    {
        private readonly string _connectionString;

        public AllergeneDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=epicurapp.db";
        }

        public List<Allergene> GetAll()
        {
            var allergenes = new List<Allergene>();
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

        public List<Allergene> GetAllergenesByClient(int clientId)
        {
            var allergenes = new List<Allergene>();
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
