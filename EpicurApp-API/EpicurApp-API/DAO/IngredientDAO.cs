using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des ingrédients dans la base de données
    /// </summary>
    public class IngredientDAO : IIngredientDAO
    {
        private readonly DatabaseConfiguration _dbConfig;

        public IngredientDAO(DatabaseConfiguration dbConfig)
        {
            _dbConfig = dbConfig;
        }

        /// <summary>
        /// Récupère tous les ingrédients
        /// </summary>
        public List<Ingredient> GetAll()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            const string query = "SELECT Id, Nom, Description FROM Ingredients ORDER BY Nom;";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ingredient ingredient = new Ingredient
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                        };

                        ingredients.Add(ingredient);
                    }
                }
            }

            return ingredients;
        }

        /// <summary>
        /// Récupère un ingrédient par son identifiant
        /// </summary>
        public Ingredient? GetById(int id)
        {
            const string query = "SELECT Id, Nom, Description FROM Ingredients WHERE Id = @Id;";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Ingredient ingredient = new Ingredient
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                            };

                            return ingredient;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Récupère les ingrédients d'un plat
        /// </summary>
        public List<Ingredient> GetIngredientsByPlatId(int platId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            const string query = @"SELECT i.Id, i.Nom, i.Description
                                  FROM Ingredients i
                                  INNER JOIN PlatIngredient pi ON i.Id = pi.IngredientId
                                  WHERE pi.PlatId = @PlatId
                                  ORDER BY i.Nom;";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlatId", platId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Ingredient ingredient = new Ingredient
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                            };

                            ingredients.Add(ingredient);
                        }
                    }
                }
            }

            return ingredients;
        }

        /// <summary>
        /// Ajoute un nouvel ingrédient
        /// </summary>
        public void Add(Ingredient ingredient)
        {
            const string query = "INSERT INTO Ingredients (Nom, Description) VALUES (@Nom, @Description);";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", ingredient.Nom);
                    command.Parameters.AddWithValue("@Description", ingredient.Description ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT last_insert_rowid();";
                    ingredient.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Associe des ingrédients à un plat
        /// </summary>
        public void AssocierIngredientsAuPlat(int platId, List<int> ingredientIds)
        {
            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                string deleteQuery = "DELETE FROM PlatIngredient WHERE PlatId = @PlatId";
                using (SqliteCommand commandDelete = new SqliteCommand(deleteQuery, connection))
                {
                    commandDelete.Parameters.AddWithValue("@PlatId", platId);
                    commandDelete.ExecuteNonQuery();
                }

                foreach (int ingredientId in ingredientIds)
                {
                    string insertQuery = "INSERT INTO PlatIngredient (PlatId, IngredientId) VALUES (@PlatId, @IngredientId)";

                    using (SqliteCommand commandInsert = new SqliteCommand(insertQuery, connection))
                    {
                        commandInsert.Parameters.AddWithValue("@PlatId", platId);
                        commandInsert.Parameters.AddWithValue("@IngredientId", ingredientId);
                        commandInsert.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
