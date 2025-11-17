using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;

namespace EpicurApp_API.DAO
{
    public class MenuDAO : IMenuDAO
    {
        private string _connectionString;

        public MenuDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=epicurapp.db";
        }

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

                using (SqliteCommand lastIdCommand = new SqliteCommand("SELECT last_insert_rowid();", connection))
                {
                    long lastId = (long)lastIdCommand.ExecuteScalar();
                    menu.Id = Convert.ToInt32(lastId);
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
                            return AvoirMenu(reader);
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
                        menus.Add(AvoirMenu(reader));
                    }
                }
            }
            return menus;
        }

        public Menu? GetDernierBrouillon()
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Date, Statut,
                    AmuseBoucheId, BoissonAperitifId, EntreeId,
                    PlatPrincipalId, VinId, FromageId, DessertId
                    FROM Menus
                    WHERE Statut = @Statut
                    ORDER BY Date DESC
                    LIMIT 1";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Statut", "Brouillon");

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return AvoirMenu(reader);
                        }
                    }
                }
            }

            return null;
        }

        public void MettreAJourMenu(Menu menu)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"UPDATE Menus SET
                    Nom = @Nom,
                    Date = @Date,
                    Statut = @Statut,
                    AmuseBoucheId = @AmuseBoucheId,
                    BoissonAperitifId = @BoissonAperitifId,
                    EntreeId = @EntreeId,
                    PlatPrincipalId = @PlatPrincipalId,
                    VinId = @VinId,
                    FromageId = @FromageId,
                    DessertId = @DessertId
                    WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", menu.Nom);
                    command.Parameters.AddWithValue("@Date", menu.Date);
                    command.Parameters.AddWithValue("@Statut", menu.Statut);
                    command.Parameters.AddWithValue("@Id", menu.Id);

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

        private static Menu AvoirMenu(SqliteDataReader reader)
        {
            Menu menu = new Menu();
            menu.Id = reader.GetInt32(0);
            menu.Nom = reader.GetString(1);
            menu.Date = reader.GetDateTime(2);
            menu.Statut = reader.GetString(3);

            menu.AmuseBoucheId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            menu.BoissonAperitifId = reader.IsDBNull(5) ? null : reader.GetInt32(5);
            menu.EntreeId = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            menu.PlatPrincipalId = reader.IsDBNull(7) ? null : reader.GetInt32(7);
            menu.VinId = reader.IsDBNull(8) ? null : reader.GetInt32(8);
            menu.FromageId = reader.IsDBNull(9) ? null : reader.GetInt32(9);
            menu.DessertId = reader.IsDBNull(10) ? null : reader.GetInt32(10);

            return menu;
        }
    }
}