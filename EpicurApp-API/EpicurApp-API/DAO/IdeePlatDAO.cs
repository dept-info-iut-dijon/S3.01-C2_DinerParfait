using EpicurAPP_Partage.Models;
using EpicurApp_API.Configuration;
using EpicurAppLogic.Interfaces;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des idées de plats.
    /// </summary>
    public class IdeePlatDAO : IIdeePlatDAO
    {
        /// <summary>
        /// String de connexion à la base de données.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Constructeur : injection de la configuration de la base de données.
        /// </summary>
        /// <param name="databaseConfiguration">la configuration actuelle de la db</param>
        public IdeePlatDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère une idée de plat par son identifiant.
        /// </summary>
        /// <returns>Idée de plat ou null si non trouvée.</returns>
        public IdeePlat? GetById(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Titre, Description, Categorie, Notes, DateCreation, RestaurantId FROM IdeesPlats WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new IdeePlat
                            {
                                Id = reader.GetInt32(0),
                                Titre = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Categorie = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Notes = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                DateCreation = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                RestaurantId = reader.GetInt32(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        /// <returns>Liste des idées de plats.</returns>
        public List<IdeePlat> GetAll()
        {
            List<IdeePlat> idees = new List<IdeePlat>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Titre, Description, Categorie, Notes, DateCreation, RestaurantId FROM IdeesPlats";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        IdeePlat idee = new IdeePlat
                        {
                            Id = reader.GetInt32(0),
                            Titre = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Categorie = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            Notes = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            DateCreation = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            RestaurantId = reader.GetInt32(6)
                        };
                        idees.Add(idee);
                    }
                }
            }

            return idees;
        }

        /// <summary>
        /// Récupère toutes les idées de plats d'un restaurant spécifique.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des idées de plats du restaurant.</returns>
        public List<IdeePlat> GetAllByRestaurantId(int restaurantId)
        {
            List<IdeePlat> idees = new List<IdeePlat>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Titre, Description, Categorie, Notes, DateCreation, RestaurantId FROM IdeesPlats WHERE RestaurantId = @RestaurantId";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            IdeePlat idee = new IdeePlat
                            {
                                Id = reader.GetInt32(0),
                                Titre = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Categorie = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Notes = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                DateCreation = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                RestaurantId = reader.GetInt32(6)
                            };
                            idees.Add(idee);
                        }
                    }
                }
            }

            return idees;
        }

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        /// <param name="idee">L'idée de plat à ajouter.</param>
        public void Ajouter(IdeePlat idee)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO IdeesPlats (Titre, Description, Categorie, Notes, RestaurantId) VALUES (@Titre, @Description, @Categorie, @Notes, @RestaurantId)";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Titre", idee.Titre ?? "");
                    command.Parameters.AddWithValue("@Description", idee.Description ?? "");
                    command.Parameters.AddWithValue("@Categorie", idee.Categorie ?? "");
                    command.Parameters.AddWithValue("@Notes", idee.Notes ?? "");
                    command.Parameters.AddWithValue("@RestaurantId", idee.RestaurantId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Modifie une idée de plat.
        /// </summary>
        /// <param name="idee">L'idée de plat à modifier.</param>
        public void Modifier(IdeePlat idee)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE IdeesPlats SET Titre = @Titre, Description = @Description, Categorie = @Categorie, Notes = @Notes WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", idee.Id);
                    command.Parameters.AddWithValue("@Titre", idee.Titre ?? "");
                    command.Parameters.AddWithValue("@Description", idee.Description ?? "");
                    command.Parameters.AddWithValue("@Categorie", idee.Categorie ?? "");
                    command.Parameters.AddWithValue("@Notes", idee.Notes ?? "");
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        /// <param name="id">L'identifiant de l'idée de plat à supprimer.</param>
        public void Supprimer(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM IdeesPlats WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}