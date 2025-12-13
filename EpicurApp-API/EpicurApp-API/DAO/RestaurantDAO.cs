using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des restaurants.
    /// </summary>
    public class RestaurantDAO : IRestaurantDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de RestaurantDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public RestaurantDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère un restaurant par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du restaurant.</param>
        /// <returns>Le restaurant trouvé ou null si non trouvé.</returns>
        public Restaurant? GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Nom, Ville
                    FROM Restaurants WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new Restaurant
                        {
                            Id = reader.GetInt32(0),
                            Nom = reader.GetString(1),
                            Ville = reader.GetString(2)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Récupère tous les restaurants.
        /// </summary>
        /// <returns>Liste de tous les restaurants.</returns>
        public List<Restaurant> GetAll()
        {
            List<Restaurant> restaurants = new List<Restaurant>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT Id, Nom, Ville FROM Restaurants";

                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            restaurants.Add(new Restaurant
                            {
                                Id = reader.GetInt32(0),
                                Nom = reader.GetString(1),
                                Ville = reader.GetString(2)
                            });
                        }
                    }
                }
            }

            return restaurants;
        }

        /// <summary>
        /// Ajoute un nouveau restaurant dans la base de données.
        /// </summary>
        /// <param name="restaurant">Le restaurant à ajouter.</param>
        public void AjouterRestaurant(Restaurant restaurant)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Restaurants
                    (Nom, Ville)
                    VALUES (@Nom, @Ville);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nom", restaurant.Nom);
                    command.Parameters.AddWithValue("@Ville", restaurant.Ville);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        restaurant.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        /// <summary>
        /// Modifie un restaurant existant.
        /// </summary>
        /// <param name="restaurant">Le restaurant avec les données mises à jour.</param>
        public void ModifierRestaurant(Restaurant restaurant)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"UPDATE Restaurants
                    SET Nom = @Nom, Ville = @Ville
                    WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", restaurant.Id);
                    command.Parameters.AddWithValue("@Nom", restaurant.Nom);
                    command.Parameters.AddWithValue("@Ville", restaurant.Ville);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Supprime un restaurant de la base de données.
        /// </summary>
        /// <param name="id">Identifiant du restaurant à supprimer.</param>
        public void SupprimerRestaurant(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Restaurants WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
