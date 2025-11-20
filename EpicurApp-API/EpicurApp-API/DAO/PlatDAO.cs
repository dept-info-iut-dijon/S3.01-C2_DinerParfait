using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using System.Linq;
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
        private readonly IIngredientDAO _ingredientDAO;

        /// <summary>
        /// Initialise une nouvelle instance de PlatDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        /// <param name="ingredientDAO">DAO des ingrédients pour la gestion des ingrédients liés aux plats.</param>
        public PlatDAO(DatabaseConfiguration databaseConfiguration, IIngredientDAO ingredientDAO)
        {
            _connexionString = databaseConfiguration.GetConnectionString();
            _ingredientDAO = ingredientDAO;
        }

        /// <summary>
        /// Récupère tous les plats de la base de données triés par catégorie et nom.
        /// </summary>
        /// <returns>Liste de tous les plats.</returns>
        public List<Plat> GetAll()
        {
            List<Plat> plats = new List<Plat>();
            const string query = "SELECT Id, Nom, Categorie FROM Plats ORDER BY Categorie, Nom;";

            using (SqliteConnection connexion = new SqliteConnection(_connexionString))
            {
                connexion.Open();
                using (SqliteCommand cmd = new SqliteCommand(query, connexion))
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var plat = MapperPlatDepuisReader(reader);
                        // Remplissage de la List<Ingredient> via le DAO injecté
                        plat.IngredientsPrincipaux = _ingredientDAO.GetIngredientsByPlatId(plat.Id);
                        plats.Add(plat);
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
            const string query = "SELECT Id, Nom, Categorie FROM Plats WHERE Id = @Id;";

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
                            // Remplissage de la liste
                            plat.IngredientsPrincipaux = _ingredientDAO.GetIngredientsByPlatId(plat.Id);
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
            // On insert le plat. La colonne IngredientsPrincipaux (texte) est laissée vide ou par défaut.
            const string query = "INSERT INTO Plats (Nom, Categorie, IngredientsPrincipaux) VALUES (@Nom, @Categorie, '');";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie.ToString());

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT last_insert_rowid();";
                    plat.Id = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            // Gestion de la table de liaison via IngredientDAO
            if (plat.IngredientsPrincipaux != null && plat.IngredientsPrincipaux.Any())
            {
                var ids = plat.IngredientsPrincipaux.Select(i => i.Id).ToList();
                _ingredientDAO.AssocierIngredientsAuPlat(plat.Id, ids);
            }
        }

        /// <summary>
        /// Met à jour les informations d'un plat existant.
        /// </summary>
        /// <param name="plat">Le plat avec les informations mises à jour.</param>
        public void Update(Plat plat)
        {
            const string query = "UPDATE Plats SET Nom = @Nom, Categorie = @Categorie WHERE Id = @Id;";

            using (SqliteConnection connection = new SqliteConnection(_connexionString))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", plat.Id);
                    command.Parameters.AddWithValue("@Nom", plat.Nom);
                    command.Parameters.AddWithValue("@Categorie", plat.Categorie.ToString());

                    command.ExecuteNonQuery();
                }
            }

            // Mise à jour des liaisons
            if (plat.IngredientsPrincipaux != null)
            {
                var ids = plat.IngredientsPrincipaux.Select(i => i.Id).ToList();
                _ingredientDAO.AssocierIngredientsAuPlat(plat.Id, ids);
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

            return new Plat
            {
                Id = reader.GetInt32(0),
                Nom = reader.GetString(1),
                Categorie = categorie,
                IngredientsPrincipaux = new List<Ingredient>()
            };
        }
    }
}