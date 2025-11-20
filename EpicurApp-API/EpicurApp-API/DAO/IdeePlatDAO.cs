using EpicurAPP_Partage.Models;
using EpicurApp_API.Configuration;
using Microsoft.Data.Sqlite;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des idées de plats.
    /// </summary>
    public class IdeePlatDAO
    {
        private readonly string _connectionString;

        public IdeePlatDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        public List<IdeePlat> GetAll()
        {
            List<IdeePlat> idees = new List<IdeePlat>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Titre, Description, Categorie, Notes, DateCreation FROM IdeesPlats";

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
                            DateCreation = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        };
                        idees.Add(idee);
                    }
                }
            }

            return idees;
        }

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        public void Ajouter(IdeePlat idee)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO IdeesPlats (Titre, Description, Categorie, Notes) VALUES (@Titre, @Description, @Categorie, @Notes)";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Titre", idee.Titre ?? "");
                    command.Parameters.AddWithValue("@Description", idee.Description ?? "");
                    command.Parameters.AddWithValue("@Categorie", idee.Categorie ?? "");
                    command.Parameters.AddWithValue("@Notes", idee.Notes ?? "");
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Modifie une idée de plat.
        /// </summary>
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