using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// Implémentation du repository client utilisant SQLite.
    /// </summary>
    public class ClientDAO : IClientDAO
    {
        private string _connectionString = "Data Source=epicurapp.db";

        public void AjouterClient(Client client)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Clients (Nom, Prenom, Email, Telephone, Allergies, Notes) VALUES (@Nom, @Prenom, @Email, @Telephone, @Allergies, @Notes)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    command.Parameters.AddWithValue("@Telephone", client.Telephone);
                    command.Parameters.AddWithValue("@Allergies", client.Allergies);
                    command.Parameters.AddWithValue("@Notes", client.Notes);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Client rechercherClientParId(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Clients WHERE Id = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null; // Aucun client avc l'ID
                        }
                        else
                        {
                            // Recuperer les données du client
                            var client = new Client
                            {

                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Email = reader.GetString(3),
                                Telephone = reader.GetString(4),
                                Allergies = reader.GetString(5),
                                Notes = reader.GetString(6)
                            };
                            return client;

                        }
                    }
                }
            }
        }
    }
}
