using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;
using System;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des menus.
    /// </summary>
    public class MenuDAO : IMenuDAO
    {
        /// <summary>
        /// Chaîne de connexion à la base de données.
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// DAO pour accéder aux plats.
        /// </summary>
        private readonly IPlatDAO _platDAO;

        /// <summary>
        /// Initialise une nouvelle instance de MenuDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        /// <param name="platDAO">DAO pour accéder aux plats.</param>
        public MenuDAO(DatabaseConfiguration databaseConfiguration, IPlatDAO platDAO)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
            _platDAO = platDAO;
        }

        /// <summary>
        /// Ajoute un nouveau menu dans la base de données.
        /// </summary>
        /// <param name="menu">Le menu à ajouter.</param>
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

                    command.Parameters.AddWithValue("@AmuseBoucheId",
                        menu.AmuseBouche?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BoissonAperitifId",
                        menu.BoissonAperitif?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EntreeId",
                        menu.Entree?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PlatPrincipalId",
                        menu.PlatPrincipal?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VinId",
                        menu.Vin?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FromageId",
                        menu.Fromage?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DessertId",
                        menu.Dessert?.Id ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Récupère un menu par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du menu.</param>
        /// <returns>Le menu trouvé ou null.</returns>
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

        /// <summary>
        /// Récupère tous les menus de la base de données.
        /// </summary>
        /// <returns>Liste de tous les menus.</returns>
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

        /// <summary>
        /// Récupère le dernier menu en statut "Brouillon".
        /// </summary>
        /// <returns>Le dernier menu brouillon ou null.</returns>
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

        /// <summary>
        /// Met à jour les informations d'un menu existant.
        /// </summary>
        /// <param name="menu">Le menu avec les informations mises à jour.</param>
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

                    command.Parameters.AddWithValue("@AmuseBoucheId",
                        menu.AmuseBouche?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BoissonAperitifId",
                        menu.BoissonAperitif?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EntreeId",
                        menu.Entree?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PlatPrincipalId",
                        menu.PlatPrincipal?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@VinId",
                        menu.Vin?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FromageId",
                        menu.Fromage?.Id ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DessertId",
                        menu.Dessert?.Id ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Associe une liste de plats à un menu.
        /// Supprime d'abord les anciennes associations puis ajoute les nouvelles.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <param name="platIds">Liste des identifiants des plats.</param>
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

        /// <summary>
        /// Méthode privée pour construire un objet Menu à partir d'un SqliteDataReader.
        /// Récupère les plats associés via leurs IDs.
        /// </summary>
        /// <param name="reader">Le reader contenant les données du menu.</param>
        /// <returns>Un objet Menu construit.</returns>
        private Menu AvoirMenu(SqliteDataReader reader)
        {
            Menu menu = new Menu();
            menu.Id = reader.GetInt32(0);
            menu.Nom = reader.GetString(1);
            menu.Date = reader.GetDateTime(2);
            menu.Statut = reader.GetString(3);

            // Récupération des plats via leurs IDs
            int? amuseBoucheId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            int? boissonAperitifId = reader.IsDBNull(5) ? null : reader.GetInt32(5);
            int? entreeId = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            int? platPrincipalId = reader.IsDBNull(7) ? null : reader.GetInt32(7);
            int? vinId = reader.IsDBNull(8) ? null : reader.GetInt32(8);
            int? fromageId = reader.IsDBNull(9) ? null : reader.GetInt32(9);
            int? dessertId = reader.IsDBNull(10) ? null : reader.GetInt32(10);

            // Chargement des plats
            menu.AmuseBouche = amuseBoucheId.HasValue ? _platDAO.GetById(amuseBoucheId.Value) : null;
            menu.BoissonAperitif = boissonAperitifId.HasValue ? _platDAO.GetById(boissonAperitifId.Value) : null;
            menu.Entree = entreeId.HasValue ? _platDAO.GetById(entreeId.Value) : null;
            menu.PlatPrincipal = platPrincipalId.HasValue ? _platDAO.GetById(platPrincipalId.Value) : null;
            menu.Vin = vinId.HasValue ? _platDAO.GetById(vinId.Value) : null;
            menu.Fromage = fromageId.HasValue ? _platDAO.GetById(fromageId.Value) : null;
            menu.Dessert = dessertId.HasValue ? _platDAO.GetById(dessertId.Value) : null;

            return menu;
        }

        /// <summary>
        /// Supprime un menu par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du menu à supprimer.</param>
        public void SupprimerMenu(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Menus WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}