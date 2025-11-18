using EpicurApp_API.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using EpicurAppLogic.Interfaces;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des plats.
    /// </summary>
    public class PlatDAO : IPlatDAO
    {
        private readonly string _connexionString;

        /// <summary>
        /// Initialise une nouvelle instance de PlatDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public PlatDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connexionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère tous les plats de la base de données triés par catégorie et nom.
        /// </summary>
        /// <returns>Liste de tous les plats.</returns>
        public List<Plat> GetAll()
        {
            List<Plat> plats = new List<Plat>();
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux FROM Plats ORDER BY Categorie, Nom;";

            using (SqliteConnection connexion = new SqliteConnection(_connexionString))
            {
                connexion.Open();
                using (SqliteCommand cmd = new SqliteCommand(query, connexion))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        plats.Add(MapperPlatDepuisReader(reader));
                    }
                }
            }

            return plats;
        }

        /// <summary>
        /// Récupère un plat par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du plat.</param>
        /// <returns>Le plat trouvé ou null.</returns>
        public Plat? GetById(int id)
        {
            Plat? plat = null;
            const string query = "SELECT Id, Nom, Categorie, IngredientsPrincipaux FROM Plats WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            plat = MapperPlatDepuisReader(reader);
                        }
                    }
                }
            }

            return plat;
        }

        /// <summary>
        /// Ajoute un nouveau plat dans la base de données.
        /// </summary>
        /// <param name="plat">Le plat à ajouter.</param>
        public void Add(Plat plat)
        {
            const string query = "INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux) VALUES (@Nom, @Categorie, @IngredientsPrincipaux);";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie.ToString());
                    command.Parameters.AddWithValue("@IngredientsPrincipaux",
                        plat.IngredientsPrincipaux != null ? string.Join(", ", plat.IngredientsPrincipaux) : string.Empty);

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT last_insert_rowid();";
                    plat.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Met à jour les informations d'un plat existant.
        /// </summary>
        /// <param name="plat">Le plat avec les informations mises à jour.</param>
        public void Update(Plat plat)
        {
            const string query = "UPDATE Plats SET Nom = @Nom, Categorie = @Categorie, IngredientsPrincipaux = @IngredientsPrincipaux WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", plat.Id);
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie.ToString());
                    command.Parameters.AddWithValue("@IngredientsPrincipaux",
                        plat.IngredientsPrincipaux != null ? string.Join(", ", plat.IngredientsPrincipaux) : string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Supprime un plat de la base de données.
        /// </summary>
        /// <param name="id">Identifiant du plat à supprimer.</param>
        public void Delete(int id)
        {
            const string query = "DELETE FROM Plats WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Méthode privée pour mapper un SqliteDataReader vers un objet Plat.
        /// Convertit la catégorie string en enum et les ingrédients string en liste.
        /// </summary>
        /// <param name="reader">Le reader contenant les données du plat.</param>
        /// <returns>Un objet Plat construit.</returns>
        private Plat MapperPlatDepuisReader(SqliteDataReader reader)
        {
            // Conversion de la catégorie string en enum
            string categorieString = reader.GetString(2);
            CategoriePlat categorie;
            if (!Enum.TryParse<CategoriePlat>(categorieString, out categorie))
            {
                // Si la conversion échoue, utiliser une valeur par défaut
                categorie = CategoriePlat.PlatPrincipal;
            }

            // Conversion des ingrédients string en liste
            string ingredientsString = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            List<string> ingredients = new List<string>();
            if (!string.IsNullOrWhiteSpace(ingredientsString))
            {
                ingredients = new List<string>(ingredientsString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
            }

            return new Plat
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                Categorie = categorie,
                IngredientsPrincipaux = ingredients
            };
        }
    }
}