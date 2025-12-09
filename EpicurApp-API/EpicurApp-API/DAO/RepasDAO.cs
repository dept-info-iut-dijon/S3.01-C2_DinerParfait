using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des repas.
    /// </summary>
    public class RepasDAO : IRepasDAO
    {
        /// <summary>
        /// Connection string vers la base de données.
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// DAO pour accéder aux menus.
        /// </summary>
        private readonly IMenuDAO _menuDAO;

        /// <summary>
        /// Initialise une nouvelle instance de RepasDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        /// <param name="menuDAO">DAO pour accéder aux menus.</param>
        public RepasDAO(DatabaseConfiguration databaseConfiguration, IMenuDAO menuDAO)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
            _menuDAO = menuDAO;
        }

        /// <summary>
        /// Récupère tous les repas d'un client spécifique avec leurs menus, triés par date décroissante.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des repas du client avec leurs menus associés.</returns>
        public List<Repas> GetRepasByClientId(int clientId)
        {
            List<Repas> repas = new List<Repas>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        r.Id, r.ClientId, r.MenuId, r.Date, r.Retours,
                        m.Id AS Menu_Id, m.Nom AS Menu_Nom, m.Note AS Menu_Note
                    FROM Repas r
                    JOIN Menus m ON r.MenuId = m.Id
                    WHERE r.ClientId = @ClientId
                    ORDER BY r.Date DESC";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Menu menu = new Menu
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Menu_Id")),
                                Nom = reader.GetString(reader.GetOrdinal("Menu_Nom")),

                                Note = reader.IsDBNull(reader.GetOrdinal("Menu_Note"))
                                       ? null
                                       : reader.GetInt32(reader.GetOrdinal("Menu_Note"))
                            };

                            var nouveauRepas = new Repas
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                ClientId = reader.GetInt32(reader.GetOrdinal("ClientId")),
                                MenuId = reader.GetInt32(reader.GetOrdinal("MenuId")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Retours = reader.IsDBNull(reader.GetOrdinal("Retours"))
                                          ? null
                                          : reader.GetString(reader.GetOrdinal("Retours")),
                                Menu = menu 
                            };

                            nouveauRepas.Note = menu.Note;
                            repas.Add(nouveauRepas);
                        }
                    }
                }
            }

            return repas;
        }

        /// <summary>
        /// Ajoute un nouveau repas dans la base de données.
        /// </summary>
        /// <param name="repas">Le repas à ajouter.</param>
        public void AjouterRepas(Repas repas)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    INSERT INTO Repas (ClientId, MenuId, Date, Retours)
                    VALUES (@ClientId, @MenuId, @Date, @Retours);
                    SELECT last_insert_rowid();";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", repas.ClientId);
                    command.Parameters.AddWithValue("@MenuId", repas.MenuId);
                    command.Parameters.AddWithValue("@Date", repas.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@Retours", repas.Retours ?? (object)DBNull.Value);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        repas.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        /// <summary>
        /// Récupère un repas par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du repas.</param>
        /// <returns>Le repas trouvé ou null.</returns>
        public Repas? GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT Id, ClientId, MenuId, Date, Retours
                    FROM Repas
                    WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Repas
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                MenuId = reader.GetInt32(2),
                                Date = DateTime.Parse(reader.GetString(3)),
                                Retours = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Met à jour les retours d'un repas existant.
        /// </summary>
        /// <param name="repasId">Identifiant du repas.</param>
        /// <param name="retours">Nouveaux retours.</param>
        public void ModifierRetours(int repasId, string retours)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE Repas
                    SET Retours = @Retours
                    WHERE Id = @Id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", repasId);
                    command.Parameters.AddWithValue("@Retours", retours ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
