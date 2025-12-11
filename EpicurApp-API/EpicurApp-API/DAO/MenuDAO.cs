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
                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert menu into Menus table
                        string menuQuery = @"INSERT INTO Menus (Nom, Statut, RestaurantId, DateCreation)
                                           VALUES (@Nom, @Statut, @RestaurantId, @DateCreation);
                                           SELECT last_insert_rowid();";

                        int menuId;
                        using (SqliteCommand command = new SqliteCommand(menuQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Nom", menu.Nom);
                            command.Parameters.AddWithValue("@Statut", menu.Statut);
                            command.Parameters.AddWithValue("@RestaurantId", menu.RestaurantId);
                            command.Parameters.AddWithValue("@DateCreation", menu.DateCreation.ToString("yyyy-MM-dd HH:mm:ss"));

                            menuId = Convert.ToInt32(command.ExecuteScalar());
                            menu.Id = menuId;
                        }

                        // Insert elements into ElementMenus table
                        if (menu.Elements != null && menu.Elements.Count > 0)
                        {
                            // Valider que tous les plats existent et appartiennent au restaurant
                            foreach (ElementMenu element in menu.Elements)
                            {
                                string checkPlatQuery = @"SELECT COUNT(*) FROM Plats WHERE Id = @PlatId AND RestaurantId = @RestaurantId";
                                using (SqliteCommand checkCommand = new SqliteCommand(checkPlatQuery, connection, transaction))
                                {
                                    checkCommand.Parameters.AddWithValue("@PlatId", element.PlatId);
                                    checkCommand.Parameters.AddWithValue("@RestaurantId", menu.RestaurantId);

                                    long count = (long)(checkCommand.ExecuteScalar() ?? 0);
                                    if (count == 0)
                                    {
                                        throw new InvalidOperationException($"Le plat avec l'ID {element.PlatId} n'existe pas ou n'appartient pas au restaurant {menu.RestaurantId}.");
                                    }
                                }
                            }

                            string elementQuery = @"INSERT INTO ElementMenus (MenuId, PlatId, Categorie, Ordre)
                                                  VALUES (@MenuId, @PlatId, @Categorie, @Ordre)";

                            foreach (ElementMenu element in menu.Elements)
                            {
                                using (SqliteCommand command = new SqliteCommand(elementQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@MenuId", menuId);
                                    command.Parameters.AddWithValue("@PlatId", element.PlatId);
                                    command.Parameters.AddWithValue("@Categorie", (int)element.Categorie);
                                    command.Parameters.AddWithValue("@Ordre", element.Ordre);

                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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

                string query = @"SELECT Id, Nom, Statut, RestaurantId, Note, Retours, DateCreation,
                                (SELECT COUNT(*) FROM Services WHERE MenuId = Menus.Id) > 0 AS EstUtilise
                                FROM Menus WHERE Id=@Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return AvoirMenu(reader, connection);
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

                string query = @"SELECT Id, Nom, Statut, RestaurantId, Note, Retours, DateCreation,
                                (SELECT COUNT(*) FROM Services WHERE MenuId = Menus.Id) > 0 AS EstUtilise
                                FROM Menus";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        menus.Add(AvoirMenu(reader, connection));
                    }
                }
            }
            return menus;
        }

        /// <summary>
        /// Récupère tous les menus d'un restaurant spécifique.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des menus du restaurant.</returns>
        public List<Menu> GetAllByRestaurantId(int restaurantId)
        {
            List<Menu> menus = new List<Menu>();
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Statut, RestaurantId, Note, Retours, DateCreation,
                                (SELECT COUNT(*) FROM Services WHERE MenuId = Menus.Id) > 0 AS EstUtilise
                                FROM Menus
                                WHERE RestaurantId = @RestaurantId";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menus.Add(AvoirMenu(reader, connection));
                        }
                    }
                }
            }
            return menus;
        }

        /// <summary>
        /// Récupère le dernier menu en statut "Brouillon" pour un restaurant donné.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Le dernier menu brouillon du restaurant ou null.</returns>
        public Menu? GetDernierBrouillon(int restaurantId)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Statut, RestaurantId, Note, Retours, DateCreation,
                    (SELECT COUNT(*) FROM Services WHERE MenuId = Menus.Id) > 0 AS EstUtilise
                    FROM Menus
                    WHERE Statut = @Statut AND RestaurantId = @RestaurantId
                    ORDER BY Id DESC
                    LIMIT 1";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Statut", "Brouillon");
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return AvoirMenu(reader, connection);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Récupère tous les menus validés d'un restaurant.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des menus validés du restaurant.</returns>
        public List<Menu> GetMenusValides(int restaurantId)
        {
            List<Menu> menus = new List<Menu>();
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Statut, RestaurantId, Note, Retours, DateCreation,
                                (SELECT COUNT(*) FROM Services WHERE MenuId = Menus.Id) > 0 AS EstUtilise
                                FROM Menus
                                WHERE RestaurantId = @RestaurantId AND Statut = @Statut
                                ORDER BY DateCreation DESC";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    command.Parameters.AddWithValue("@Statut", "Validé");

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menus.Add(AvoirMenu(reader, connection));
                        }
                    }
                }
            }
            return menus;
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
                using (SqliteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update Menus table
                        string updateMenuQuery = @"UPDATE Menus SET
                            Nom = @Nom,
                            Statut = @Statut,
                            Note = @Note,
                            Retours = @Retours
                            WHERE Id = @Id";

                        using (SqliteCommand command = new SqliteCommand(updateMenuQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Nom", menu.Nom);
                            command.Parameters.AddWithValue("@Statut", menu.Statut);
                            command.Parameters.AddWithValue("@Id", menu.Id);
                            command.Parameters.AddWithValue("@Note",
                                menu.Note.HasValue ? (object)menu.Note.Value : DBNull.Value);
                            command.Parameters.AddWithValue("@Retours",
                                menu.Retours ?? (object)DBNull.Value);

                            command.ExecuteNonQuery();
                        }

                        // Delete existing elements
                        string deleteQuery = "DELETE FROM ElementMenus WHERE MenuId=@MenuId";
                        using (SqliteCommand command = new SqliteCommand(deleteQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MenuId", menu.Id);
                            command.ExecuteNonQuery();
                        }

                        // Insert new elements
                        if (menu.Elements != null && menu.Elements.Count > 0)
                        {
                            // Valider que tous les plats existent et appartiennent au restaurant
                            foreach (ElementMenu element in menu.Elements)
                            {
                                string checkPlatQuery = @"SELECT COUNT(*) FROM Plats WHERE Id = @PlatId AND RestaurantId = @RestaurantId";
                                using (SqliteCommand checkCommand = new SqliteCommand(checkPlatQuery, connection, transaction))
                                {
                                    checkCommand.Parameters.AddWithValue("@PlatId", element.PlatId);
                                    checkCommand.Parameters.AddWithValue("@RestaurantId", menu.RestaurantId);

                                    long count = (long)(checkCommand.ExecuteScalar() ?? 0);
                                    if (count == 0)
                                    {
                                        throw new InvalidOperationException($"Le plat avec l'ID {element.PlatId} n'existe pas ou n'appartient pas au restaurant {menu.RestaurantId}.");
                                    }
                                }
                            }

                            string insertElementQuery = @"INSERT INTO ElementMenus (MenuId, PlatId, Categorie, Ordre)
                                                        VALUES (@MenuId, @PlatId, @Categorie, @Ordre)";

                            foreach (ElementMenu element in menu.Elements)
                            {
                                using (SqliteCommand command = new SqliteCommand(insertElementQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@MenuId", menu.Id);
                                    command.Parameters.AddWithValue("@PlatId", element.PlatId);
                                    command.Parameters.AddWithValue("@Categorie", (int)element.Categorie);
                                    command.Parameters.AddWithValue("@Ordre", element.Ordre);

                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
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
        /// Récupère les éléments associés depuis la table ElementMenus.
        /// </summary>
        /// <param name="reader">Le reader contenant les données du menu.</param>
        /// <param name="connection">Connexion à la base de données pour charger les éléments.</param>
        /// <returns>Un objet Menu construit.</returns>
        private Menu AvoirMenu(SqliteDataReader reader, SqliteConnection connection)
        {
            Menu menu = new Menu();
            menu.Id = reader.GetInt32(0);
            menu.Nom = reader.GetString(1);
            menu.Statut = reader.GetString(2);
            menu.RestaurantId = reader.GetInt32(3);
            menu.Note = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            menu.Retours = reader.IsDBNull(5) ? null : reader.GetString(5);
            menu.DateCreation = reader.IsDBNull(6) ? DateTime.Now : DateTime.Parse(reader.GetString(6));
            menu.EstUtilise = reader.GetBoolean(7);

            // Load elements from ElementMenus table
            menu.Elements = new List<ElementMenu>();

            string elementQuery = @"SELECT Id, MenuId, PlatId, Categorie, Ordre
                                  FROM ElementMenus
                                  WHERE MenuId = @MenuId
                                  ORDER BY Categorie, Ordre";

            using (SqliteCommand elementCommand = new SqliteCommand(elementQuery, connection))
            {
                elementCommand.Parameters.AddWithValue("@MenuId", menu.Id);

                using (SqliteDataReader elementReader = elementCommand.ExecuteReader())
                {
                    while (elementReader.Read())
                    {
                        int elementId = elementReader.GetInt32(0);
                        int menuId = elementReader.GetInt32(1);
                        int platId = elementReader.GetInt32(2);
                        int categorieInt = elementReader.GetInt32(3);
                        int ordre = elementReader.GetInt32(4);

                        // Load the Plat
                        Plat? plat = _platDAO.GetById(platId);

                        if (plat != null)
                        {
                            ElementMenu element = new ElementMenu
                            {
                                Id = elementId,
                                MenuId = menuId,
                                PlatId = platId,
                                Plat = plat,
                                Categorie = (CategoriePlat)categorieInt,
                                Ordre = ordre
                            };

                            menu.Elements.Add(element);
                        }
                    }
                }
            }

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

        /// <summary>
        /// Vérifie si un menu est associé à un service dans les prochaines 24 heures.
        /// </summary>
        /// <param name="menuId">Identifiant du menu à vérifier.</param>
        /// <returns>True si le menu a un service dans les 24h, False sinon.</returns>
        public bool AServiceDansLes24Heures(int menuId)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT COUNT(*) FROM Services
                                WHERE MenuId = @MenuId
                                AND datetime(Date) >= datetime('now')
                                AND datetime(Date) <= datetime('now', '+24 hours')";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    long count = (long)(command.ExecuteScalar() ?? 0);
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Vérifie si un menu est utilisé dans un service.
        /// </summary>
        /// <param name="menuId">Id du menu à vérifier.</param>
        /// <returns>True si le menu est assigné à au moins un service.</returns>
        public bool EstUtilise(int menuId)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Services WHERE MenuId = @MenuId";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    long count = (long)(command.ExecuteScalar() ?? 0);
                    return count > 0;
                }
            }
        }
    }
}
