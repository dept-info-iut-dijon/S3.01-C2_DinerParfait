using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des plats dans la base de données
    /// </summary>
    public class PlatDAO : IPlatDAO
    {
        private readonly DatabaseConfiguration _dbConfig;
        private readonly IngredientDAO _ingredientDAO;

        public PlatDAO(DatabaseConfiguration dbConfig, IngredientDAO ingredientDAO)
        {
            _dbConfig = dbConfig;
            _ingredientDAO = ingredientDAO;
        }

        public List<Plat> GetAll()
        {
            List<Plat> plats = new List<Plat>();
            const string query = "SELECT Id, Nom, Categorie FROM Plats ORDER BY Categorie, Nom;";

            using (SqliteConnection connexion = _dbConfig.CreateConnection())
            {
                connexion.Open();

                using (SqliteCommand cmd = new SqliteCommand(query, connexion))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int platId = reader.GetInt32(0);

                        Plat plat = new Plat();
                        plat.Id = platId;
                        plat.Nom = reader.GetString(1);
                        plat.Categorie = (CategorieEnum)reader.GetInt32(2);

                        // Charger les ingrédients du plat
                        List<Ingredient> ingredients = _ingredientDAO.GetIngredientsByPlatId(platId);
                        plat.Ingredients = ingredients;

                        plats.Add(plat);
                    }
                }
            }

            return plats;
        }

        public Plat? GetById(int id)
        {
            Plat? plat = null;
            const string query = "SELECT Id, Nom, Categorie FROM Plats WHERE Id = @Id;";

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
                            int platId = reader.GetInt32(0);

                            plat = new Plat();
                            plat.Id = platId;
                            plat.Nom = reader.GetString(1);
                            plat.Categorie = (CategorieEnum)reader.GetInt32(2);

                            List<Ingredient> ingredients = _ingredientDAO.GetIngredientsByPlatId(platId);
                            plat.Ingredients = ingredients;
                        }
                    }
                }
            }

            return plat;
        }

        public void Add(Plat plat)
        {
            const string query = "INSERT INTO Plats (Nom, Categorie) VALUES (@Nom, @Categorie);";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", (int)plat.Categorie);

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT last_insert_rowid();";
                    long lastId = (long)command.ExecuteScalar();
                    plat.Id = Convert.ToInt32(lastId);
                }

                // Associer les ingrédients si présents
                if (plat.Ingredients != null && plat.Ingredients.Count > 0)
                {
                    List<int> ingredientIds = new List<int>();
                    foreach (Ingredient ingredient in plat.Ingredients)
                    {
                        ingredientIds.Add(ingredient.Id);
                    }

                    _ingredientDAO.AssocierIngredientsAuPlat(plat.Id, ingredientIds);
                }
            }
        }

        public void Update(Plat plat)
        {
            const string query = "UPDATE Plats SET Nom = @Nom, Categorie = @Categorie WHERE Id = @Id;";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", plat.Id);
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", (int)plat.Categorie);

                    command.ExecuteNonQuery();
                }

                // Mettre à jour les ingrédients
                if (plat.Ingredients != null && plat.Ingredients.Count > 0)
                {
                    List<int> ingredientIds = new List<int>();
                    foreach (Ingredient ingredient in plat.Ingredients)
                    {
                        ingredientIds.Add(ingredient.Id);
                    }

                    _ingredientDAO.AssocierIngredientsAuPlat(plat.Id, ingredientIds);
                }
            }
        }

        public void Delete(int id)
        {
            const string query = "DELETE FROM Plats WHERE Id = @Id;";

            using (SqliteConnection connection = _dbConfig.CreateConnection())
            {
                connection.Open();

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
