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

        public async Task<Client> GetByIdWithHistoryAsync(int id)
        {
            Client client = null;

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                const string clientQuery = "SELECT Id, Nom, Prenom, Email,Telephone, Allergies, Notes FROM Clients WHERE Id=@Id";
                using(var cmdClient=new SqliteCommand(clientQuery, connection))
                {
                    cmdClient.Parameters.AddWithValue("@Id", id);
                    using(var reader = await cmdClient.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            client = new Client
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Prenom = reader.GetString(2),
                                Email = reader.GetString(3),
                                Telephone = reader.GetString(4),
                                Allergies = reader.GetString(5),
                                Notes = reader.GetString(6)
                            };
                        }
                    }
                }

                if (client == null) return null;

                const string historyQuery = @"SELECT m.Id,m.Date,m.Statut FROM Menus m JOIN ClientMenu cm ON m.Id = cm.MenuId WHERE cm.ClientId = @ClientId ORDER BY m.Date DESC";

                using (var cmdHistory = new SqliteCommand(historyQuery, connection))
                {
                    cmdHistory.Parameters.AddWithValue("@ClientId",id);
                    using(var reader = await cmdHistory.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
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
