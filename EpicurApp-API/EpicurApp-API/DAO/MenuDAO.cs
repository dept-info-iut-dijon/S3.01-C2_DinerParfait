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
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Menus 
                    (Nom, Date, Statut, AmuseBoucheId, BoissonAperitifId, EntreeId, 
                     PlatPrincipalId, VinId, FromageId, DessertId) 
                    VALUES 
                    (@Nom, @Date, @Statut, @AmuseBoucheId, @BoissonAperitifId, @EntreeId, 
                     @PlatPrincipalId, @VinId, @FromageId, @DessertId)";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", menu.Nom);
                    command.Parameters.AddWithValue("@Date", menu.Date);
                    command.Parameters.AddWithValue("@Statut", menu.Statut);

                    if (menu.AmuseBoucheId == null)
                        command.Parameters.AddWithValue("@AmuseBoucheId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@AmuseBoucheId", menu.AmuseBoucheId);

                    if (menu.BoissonAperitifId == null)
                        command.Parameters.AddWithValue("@BoissonAperitifId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@BoissonAperitifId", menu.BoissonAperitifId);

                    if (menu.EntreeId == null)
                        command.Parameters.AddWithValue("@EntreeId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@EntreeId", menu.EntreeId);

                    if (menu.PlatPrincipalId == null)
                        command.Parameters.AddWithValue("@PlatPrincipalId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@PlatPrincipalId", menu.PlatPrincipalId);

                    if (menu.VinId == null)
                        command.Parameters.AddWithValue("@VinId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@VinId", menu.VinId);

                    if (menu.FromageId == null)
                        command.Parameters.AddWithValue("@FromageId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@FromageId", menu.FromageId);

                    if (menu.DessertId == null)
                        command.Parameters.AddWithValue("@DessertId", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@DessertId", menu.DessertId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Menu? GetById(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Date, Statut, 
                    AmuseBoucheId, BoissonAperitifId, EntreeId, 
                    PlatPrincipalId, VinId, FromageId, DessertId 
                    FROM Menus WHERE Id=@Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Menu menu = new Menu();
                            menu.Id = reader.GetInt32(0);
                            menu.Nom = reader.GetString(1);
                            menu.Date = reader.GetDateTime(2);
                            menu.Statut = reader.GetString(3);

                            // Lecture des IDs avec gestion des NULL
                            if (reader.IsDBNull(4))
                                menu.AmuseBoucheId = null;
                            else
                                menu.AmuseBoucheId = reader.GetInt32(4);

                            if (reader.IsDBNull(5))
                                menu.BoissonAperitifId = null;
                            else
                                menu.BoissonAperitifId = reader.GetInt32(5);

                            if (reader.IsDBNull(6))
                                menu.EntreeId = null;
                            else
                                menu.EntreeId = reader.GetInt32(6);

                            if (reader.IsDBNull(7))
                                menu.PlatPrincipalId = null;
                            else
                                menu.PlatPrincipalId = reader.GetInt32(7);

                            if (reader.IsDBNull(8))
                                menu.VinId = null;
                            else
                                menu.VinId = reader.GetInt32(8);

                            if (reader.IsDBNull(9))
                                menu.FromageId = null;
                            else
                                menu.FromageId = reader.GetInt32(9);

                            if (reader.IsDBNull(10))
                                menu.DessertId = null;
                            else
                                menu.DessertId = reader.GetInt32(10);

                            return menu;
                        }
                    }
                }
            }
            return null;
        }

        public List<Menu> GetAll()
        {
            List<Menu> menus = new List<Menu>();
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Date, Statut, 
                    AmuseBoucheId, BoissonAperitifId, EntreeId, 
                    PlatPrincipalId, VinId, FromageId, DessertId 
                    FROM Menus";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Menu menu = new Menu();
                        menu.Id = reader.GetInt32(0);
                        menu.Nom = reader.GetString(1);
                        menu.Date = reader.GetDateTime(2);
                        menu.Statut = reader.GetString(3);


                        if (reader.IsDBNull(4))
                            menu.AmuseBoucheId = null;
                        else
                            menu.AmuseBoucheId = reader.GetInt32(4);

                        if (reader.IsDBNull(5))
                            menu.BoissonAperitifId = null;
                        else
                            menu.BoissonAperitifId = reader.GetInt32(5);

                        if (reader.IsDBNull(6))
                            menu.EntreeId = null;
                        else
                            menu.EntreeId = reader.GetInt32(6);

                        if (reader.IsDBNull(7))
                            menu.PlatPrincipalId = null;
                        else
                            menu.PlatPrincipalId = reader.GetInt32(7);

                        if (reader.IsDBNull(8))
                            menu.VinId = null;
                        else
                            menu.VinId = reader.GetInt32(8);

                        if (reader.IsDBNull(9))
                            menu.FromageId = null;
                        else
                            menu.FromageId = reader.GetInt32(9);

                        if (reader.IsDBNull(10))
                            menu.DessertId = null;
                        else
                            menu.DessertId = reader.GetInt32(10);

                        menus.Add(menu);
                    }
                }
            }
            return menus;
        }

        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM MenuPlat WHERE MenuId=@MenuId";
                using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    command.ExecuteNonQuery();
                }

                foreach (int platId in platIds)
                {
                    string insertQuery = "INSERT INTO MenuPlat (MenuId, PlatId) VALUES (@MenuId, @PlatId)";
                    using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
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