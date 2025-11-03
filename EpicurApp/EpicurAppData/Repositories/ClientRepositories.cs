using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace EpicurAppData.Repositories
{
    /// <summary>
    /// Implémentation du repository client utilisant SQLite.
    /// </summary>
    public class ClientRepository : IClientRepository
    {
        private string _connectionString = "Data Source=epicurapp.db";

        public void AjouterClient(Client client)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Clients (Nom, Prenom, Email, Telephone) VALUES (@Nom, @Prenom, @Email, @Telephone)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", client.Nom);
                    command.Parameters.AddWithValue("@Prenom", client.Prenom);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    command.Parameters.AddWithValue("@Telephone", client.Telephone);
                    command.Parameters.AddWithValue("@Allergies", client.Allergies);
                    command.Parameters.AddWithValue("@Note", client.Note);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
