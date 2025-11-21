using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using static EpicurAPP_Partage.Models.Ingredient;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// Implementation du DAO pour la gestion des ingrédients.
    /// </summary>
    public class IngredientDAO : IIngredientDAO
    {
        /// <summary>
        /// String de connexion à la base de données.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Constructeur : injection de la configuration de la base de données.
        /// </summary>
        /// <param name="dbConfig">Configuration actuelle de la db</param>
        public IngredientDAO(DatabaseConfiguration dbConfig)
        {
            _connectionString = dbConfig.GetConnectionString();
        }

        /// <summary>
        /// Récupère tous les ingrédients
        /// </summary>
        public List<Ingredient> GetAll()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            const string query = "SELECT Id, Nom, Description, Categorie FROM Ingredients ORDER BY Nom;";

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredients.Add(MapperIngredient(reader));
                    }
                }
            }
            return ingredients;
        }

        /// <summary>
        /// Récupère un ingrédient par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'ingrédient</param>
        public Ingredient? GetById(int id)
        {
            const string query = "SELECT Id, Nom, Description, Categorie FROM Ingredients WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapperIngredient(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Récupère les ingrédients d'un plat
        /// </summary>
        /// <param name="platId">Identifiant du plat</param>
        /// <returns>Liste des ingrédients</returns>
        public List<Ingredient> GetIngredientsByPlatId(int platId)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            // Requête avec jointure pour récupérer les ingrédients liés au plat
            const string query = @"SELECT i.Id, i.Nom, i.Description, i.Categorie
                                  FROM Ingredients i
                                  INNER JOIN PlatIngredient pi ON i.Id = pi.IngredientId
                                  WHERE pi.PlatId = @PlatId
                                  ORDER BY i.Nom;";

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlatId", platId);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingredients.Add(MapperIngredient(reader));
                        }
                    }
                }
            }
            return ingredients;
        }

        /// <summary>
        /// Ajoute un nouvel ingrédient
        /// </summary>
        /// <param name="ingredient">L'ingrédient à ajouter</param>
        public void Add(Ingredient ingredient)
        {
            const string query = "INSERT INTO Ingredients (Nom, Description, Categorie) VALUES (@Nom, @Description, @Categorie);";

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", ingredient.Nom);
                    command.Parameters.AddWithValue("@Description", ingredient.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Categorie", ingredient.Categorie.ToString());

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT last_insert_rowid();";
                    ingredient.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Associe des ingrédients à un plat
        /// </summary>
        /// <param name="platId">Identifiant du plat</param>
        /// <param name="ingredientIds">Liste des identifiants des ingrédients à associer</param>
        public void AssocierIngredientsAuPlat(int platId, List<int> ingredientIds)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
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


        /// <summary>
        /// Permet de mapper un enregistrement de la base de données à un objet Ingredient
        /// </summary>
        /// <param name="reader">le reader sql</param>
        /// <returns>L'ingredient mappe</returns>
        private Ingredient MapperIngredient(SqliteDataReader reader)
        {
            CategorieIngredient categorie = CategorieIngredient.Autre;
            if (!reader.IsDBNull(3))
            {
                Enum.TryParse(reader.GetString(3), out categorie);
            }

            return new Ingredient
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Categorie = categorie
            };
        }
    }
}