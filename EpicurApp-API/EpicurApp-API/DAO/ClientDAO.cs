using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// Implémentation du repository client utilisant SQLite.
    /// </summary>
    public class ClientDAO : IClientDAO
    {
        private readonly string _connectionString;

        public ClientDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=epicurapp.db";
        }

        public void AjouterClient(Client client)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Clients (Nom, Prenom, Email, Telephone, Allergies, Notes) VALUES (@Nom, @Prenom, @Email, @Telephone, @Allergies, @Notes)";
                using (SqliteCommand command = new SqliteCommand(query, connection))
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
    }
}
