using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des utilisateurs.
    /// </summary>
    public class UtilisateurDAO : IUtilisateurDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de UtilisateurDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public UtilisateurDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère un utilisateur par son email.
        /// </summary>
        /// <param name="email">Email de l'utilisateur.</param>
        /// <returns>L'utilisateur trouvé ou null si non trouvé.</returns>
        public Utilisateur? GetByEmail(string email)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Email, PasswordHash, RestaurantId
                    FROM Utilisateurs WHERE Email = @Email";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new Utilisateur
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            RestaurantId = reader.GetInt32(3)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Récupère un utilisateur par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur.</param>
        /// <returns>L'utilisateur trouvé ou null si non trouvé.</returns>
        public Utilisateur? GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Email, PasswordHash, RestaurantId
                    FROM Utilisateurs WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new Utilisateur
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            RestaurantId = reader.GetInt32(3)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Récupère tous les utilisateurs d'un restaurant.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des utilisateurs du restaurant.</returns>
        public List<Utilisateur> GetByRestaurantId(int restaurantId)
        {
            List<Utilisateur> utilisateurs = new List<Utilisateur>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Email, PasswordHash, RestaurantId
                    FROM Utilisateurs WHERE RestaurantId = @RestaurantId";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            utilisateurs.Add(new Utilisateur
                            {
                                Id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                PasswordHash = reader.GetString(2),
                                RestaurantId = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }

            return utilisateurs;
        }

        /// <summary>
        /// Ajoute un nouvel utilisateur dans la base de données.
        /// </summary>
        /// <param name="utilisateur">L'utilisateur à ajouter.</param>
        public void AjouterUtilisateur(Utilisateur utilisateur)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Utilisateurs
                    (Email, PasswordHash, RestaurantId)
                    VALUES (@Email, @PasswordHash, @RestaurantId);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", utilisateur.Email);
                    command.Parameters.AddWithValue("@PasswordHash", utilisateur.PasswordHash);
                    command.Parameters.AddWithValue("@RestaurantId", utilisateur.RestaurantId);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        utilisateur.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        /// <summary>
        /// Modifie un utilisateur existant.
        /// </summary>
        /// <param name="utilisateur">L'utilisateur avec les données mises à jour.</param>
        public void ModifierUtilisateur(Utilisateur utilisateur)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"UPDATE Utilisateurs
                    SET Email = @Email, PasswordHash = @PasswordHash, RestaurantId = @RestaurantId
                    WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", utilisateur.Id);
                    command.Parameters.AddWithValue("@Email", utilisateur.Email);
                    command.Parameters.AddWithValue("@PasswordHash", utilisateur.PasswordHash);
                    command.Parameters.AddWithValue("@RestaurantId", utilisateur.RestaurantId);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Supprime un utilisateur de la base de données.
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur à supprimer.</param>
        public void SupprimerUtilisateur(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Utilisateurs WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
