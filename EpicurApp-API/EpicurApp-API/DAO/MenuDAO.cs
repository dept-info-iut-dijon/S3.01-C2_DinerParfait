using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    public class MenuDAO : IMenuDAO
    {
        private string _connectionString = "Data Source=epicurapp.db";

        public void AjouterMenu(Menu menu)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Menus (Nom, Date, Statut, CoutGlobal, TempsPreparationMinutes) " +
                               "VALUES (@Nom, @Date, @Statut, @CoutGlobal, @TempsPreparationMinutes);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", menu.Nom);
                    command.Parameters.AddWithValue("@Date", menu.Date);
                    command.Parameters.AddWithValue("@Statut", menu.Statut);
                    command.Parameters.AddWithValue("@CoutGlobal", menu.CoutGlobal);
                    command.Parameters.AddWithValue("@TempsPreparationMinutes", menu.TempsPreparationMinutes);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Menu? GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT Id, Nom, Date, Statut, CoutGlobal, TempsPreparationMinutes FROM Menus WHERE Id=@Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Menu
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Date = reader.GetDateTime(2),
                                Statut = reader.GetString(3),
                                CoutGlobal = reader.GetDecimal(4),
                                TempsPreparationMinutes = reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Menu> GetAll()
        {
            var menus = new List<Menu>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT Id, Nom, Date, Statut, CoutGlobal, TempsPreparationMinutes FROM Menus";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        menus.Add(new Menu
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Date = reader.GetDateTime(2),
                            Statut = reader.GetString(3),
                            CoutGlobal = reader.GetDecimal(4),
                            TempsPreparationMinutes = reader.GetInt32(5)
                        });
                    }
                }
            }
            return menus;
        }

        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

               
                string deleteQuery = "DELETE FROM MenuPlat WHERE MenuId=@MenuId";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    command.ExecuteNonQuery();
                }

                
                foreach (var platId in platIds)
                {
                    string insertQuery = "INSERT INTO MenuPlat (MenuId, PlatId) VALUES (@MenuId, @PlatId)";
                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MenuId", menuId);
                        command.Parameters.AddWithValue("@PlatId", platId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}

