using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;
using EpicurAppLogic.Interfaces;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des allergènes.
    /// </summary>
    public class AllergeneDAO : IAllergeneDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de AllergeneDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public AllergeneDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère tous les allergènes de la base de données.
        /// </summary>
        /// <returns>Liste de tous les allergènes.</returns>
        public List<Allergene> GetAll()
        {
            List<Allergene> allergenes = new List<Allergene>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Nom, Description FROM Allergenes";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allergenes.Add(new Allergene
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                        });
                    }
                }
            }
            return allergenes;
        }

        /// <summary>
        /// Récupère les allergènes associés à un client spécifique.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des allergènes du client.</returns>
        public List<Allergene> GetAllergenesByClient(int clientId)
        {
            List<Allergene> allergenes = new List<Allergene>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT a.Id, a.Nom, a.Description 
                                FROM Allergenes a
                                INNER JOIN ClientAllergene ca ON a.Id = ca.AllergeneId
                                WHERE ca.ClientId = @ClientId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allergenes.Add(new Allergene
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return allergenes;
        }

        /// <summary>
        /// Associe une liste d'allergènes à un client.
        /// Supprime d'abord les anciennes associations puis ajoute les nouvelles.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes à associer.</param>
        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                
                string deleteQuery = "DELETE FROM ClientAllergene WHERE ClientId = @ClientId";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.ExecuteNonQuery();
                }

                foreach (var allergeneId in allergeneIds)
                {
                    string insertQuery = "INSERT INTO ClientAllergene (ClientId, AllergeneId) VALUES (@ClientId, @AllergeneId)";
                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientId", clientId);
                        command.Parameters.AddWithValue("@AllergeneId", allergeneId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Ajoute un nouvel allergène à la base de données.
        /// </summary>
        /// <param name="allergene">L'allergène à ajouter.</param>
        public void AjouterAllergene(Allergene allergene)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Allergenes (Nom, Description) VALUES (@Nom, @Description)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", allergene.Nom);
                    command.Parameters.AddWithValue("@Description", allergene.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Récupère les allergènes présents dans les ingrédients d'un menu.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des allergènes présents dans le menu.</returns>
        public List<Allergene> GetAllergenesParMenu(int menuId)
        {
            List<Allergene> allergenes = new List<Allergene>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                // Requête qui récupère tous les allergènes liés aux ingrédients des plats du menu
                string query = @"
                    SELECT DISTINCT a.Id, a.Nom, a.Description 
                    FROM Allergenes a
                    INNER JOIN IngredientAllergene ia ON a.Id = ia.AllergeneId
                    INNER JOIN Ingredients i ON ia.IngredientId = i.Id
                    INNER JOIN PlatIngredient pi ON i.Id = pi.IngredientId
                    INNER JOIN Plats p ON pi.PlatId = p.Id
                    INNER JOIN ElementMenus em ON p.Id = em.PlatId
                    WHERE em.MenuId = @MenuId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allergenes.Add(new Allergene
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return allergenes;
        }

        /// <summary>
        /// Récupère les ingrédients d'un menu qui contiennent un allergène spécifique.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <param name="allergeneId">Identifiant de l'allergène.</param>
        /// <returns>Liste des ingrédients concernés.</returns>
        public List<Ingredient> GetIngredientsByMenuAndAllergene(int menuId, int allergeneId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT DISTINCT i.Id, i.Nom, i.Description
                    FROM Ingredients i
                    INNER JOIN IngredientAllergene ia ON i.Id = ia.IngredientId
                    INNER JOIN PlatIngredient pi ON i.Id = pi.IngredientId
                    INNER JOIN Plats p ON pi.PlatId = p.Id
                    INNER JOIN ElementMenus em ON p.Id = em.PlatId
                    WHERE em.MenuId = @MenuId AND ia.AllergeneId = @AllergeneId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuId", menuId);
                    command.Parameters.AddWithValue("@AllergeneId", allergeneId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingredients.Add(new Ingredient
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return ingredients;
        }
    }
}
