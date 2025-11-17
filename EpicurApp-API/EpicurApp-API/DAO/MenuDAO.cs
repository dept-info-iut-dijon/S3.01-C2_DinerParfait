using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des menus dans la base de données
    /// </summary>
    public class MenuDAO : IMenuDAO
    {
        private readonly DatabaseConfiguration _dbConfig;
        private readonly PlatDAO _platDAO;

        public MenuDAO(DatabaseConfiguration dbConfig, PlatDAO platDAO)
        {
            _dbConfig = dbConfig;
            _platDAO = platDAO;
        }

        public void AjouterMenu(Menu menu)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
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

                    command.Parameters.AddWithValue("@AmuseBoucheId", menu.AmuseBouche?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BoissonAperitifId", menu.BoissonAperitif?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EntreeId", menu.Entree?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PlatPrincipalId", menu.PlatPrincipal?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VinId", menu.Vin?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FromageId", menu.Fromage?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DessertId", menu.Dessert?.Id ?? (object)DBNull.Value);

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
            using (SqliteConnection connection = _dbConfig.CreateConnection())
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
                            return ConstruireMenu(reader);
                        }
                    }
                }
            }
            return null;
        }

        public List<Menu> GetAll()
        {
            List<Menu> menus = new List<Menu>();

            using (SqliteConnection connection = _dbConfig.CreateConnection())
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
                        Menu menu = ConstruireMenu(reader);
                        menus.Add(menu);
                    }
                }
            }

            return menus;
        }

        public Menu? GetDernierBrouillon()
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
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
                            return ConstruireMenu(reader);
                        }
                    }
                }
            }

            return null;
        }

        public void MettreAJourMenu(Menu menu)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
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

                    command.Parameters.AddWithValue("@AmuseBoucheId", menu.AmuseBouche?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BoissonAperitifId", menu.BoissonAperitif?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EntreeId", menu.Entree?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PlatPrincipalId", menu.PlatPrincipal?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VinId", menu.Vin?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FromageId", menu.Fromage?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DessertId", menu.Dessert?.Id ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string deleteQuery = "DELETE FROM MenuPlat WHERE MenuId=@MenuId";
                using (SqliteCommand commandDelete = new SqliteCommand(deleteQuery, connection))
                {
                    commandDelete.Parameters.AddWithValue("@MenuId", menuId);
                    commandDelete.ExecuteNonQuery();
                }

                foreach (int platId in platIds)
                {
                    string insertQuery = "INSERT INTO MenuPlat (MenuId, PlatId) VALUES (@MenuId, @PlatId)";
                    using (SqliteCommand commandInsert = new SqliteCommand(insertQuery, connection))
                    {
                        commandInsert.Parameters.AddWithValue("@MenuId", menuId);
                        commandInsert.Parameters.AddWithValue("@PlatId", platId);
                        commandInsert.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Crée un objet Menu à partir d'un reader
        /// </summary>
        private Menu ConstruireMenu(SqliteDataReader reader)
        {
            Menu menu = new Menu();
            menu.Id = reader.GetInt32(0);
            menu.Nom = reader.GetString(1);
            menu.Date = reader.GetDateTime(2);
            menu.Statut = reader.GetString(3);

            int? amuseBoucheId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            int? boissonAperitifId = reader.IsDBNull(5) ? null : reader.GetInt32(5);
            int? entreeId = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            int? platPrincipalId = reader.IsDBNull(7) ? null : reader.GetInt32(7);
            int? vinId = reader.IsDBNull(8) ? null : reader.GetInt32(8);
            int? fromageId = reader.IsDBNull(9) ? null : reader.GetInt32(9);
            int? dessertId = reader.IsDBNull(10) ? null : reader.GetInt32(10);

            menu.AmuseBouche = amuseBoucheId.HasValue ? _platDAO.GetById(amuseBoucheId.Value) : null;
            menu.BoissonAperitif = boissonAperitifId.HasValue ? _platDAO.GetById(boissonAperitifId.Value) : null;
            menu.Entree = entreeId.HasValue ? _platDAO.GetById(entreeId.Value) : null;
            menu.PlatPrincipal = platPrincipalId.HasValue ? _platDAO.GetById(platPrincipalId.Value) : null;
            menu.Vin = vinId.HasValue ? _platDAO.GetById(vinId.Value) : null;
            menu.Fromage = fromageId.HasValue ? _platDAO.GetById(fromageId.Value) : null;
            menu.Dessert = dessertId.HasValue ? _platDAO.GetById(dessertId.Value) : null;

            return menu;
        }
    }
}
