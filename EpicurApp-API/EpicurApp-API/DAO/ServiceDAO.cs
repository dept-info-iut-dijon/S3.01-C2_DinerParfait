using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des services.
    /// </summary>
    public class ServiceDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de ServiceDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public ServiceDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Ajoute un nouveau service dans la base de données.
        /// </summary>
        /// <param name="service">Le service à ajouter.</param>
        /// <param name="restaurantId">Identifiant du restaurant pour valider que le menu appartient bien à ce restaurant.</param>
        public void AjouterService(Service service, int restaurantId)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // V�rifier que le menu existe et appartient au restaurant
                string checkQuery = @"SELECT COUNT(*) FROM Menus WHERE Id = @MenuId AND RestaurantId = @RestaurantId";
                using (SqliteCommand checkCommand = new SqliteCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@MenuId", service.MenuId);
                    checkCommand.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    long count = (long)(checkCommand.ExecuteScalar() ?? 0);
                    if (count == 0)
                    {
                        throw new InvalidOperationException($"Le menu avec l'ID {service.MenuId} n'existe pas ou n'appartient pas au restaurant {restaurantId}.");
                    }
                }

                string query = @"INSERT INTO Services
                    (Date, MidiSoir, MenuId, Statut)
                    VALUES (@Date, @MidiSoir, @MenuId, @Statut);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", service.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@MidiSoir", service.MidiSoir);
                    command.Parameters.AddWithValue("@MenuId", service.MenuId);
                    command.Parameters.AddWithValue("@Statut", service.Statut);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        service.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        /// <summary>
        /// Récupère tous les services pour une date donnée et un restaurant donné.
        /// </summary>
        /// <param name="date">Date pour laquelle récupérer les services.</param>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des services trouvés.</returns>
        public List<Service> GetServicesParDate(DateTime date, int restaurantId)
        {
            List<Service> services = new List<Service>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT s.Id, s.Date, s.MidiSoir, s.MenuId, s.Statut
                    FROM Services s
                    INNER JOIN Menus m ON s.MenuId = m.Id
                    WHERE DATE(s.Date) = DATE(@Date)
                    AND m.RestaurantId = @RestaurantId
                    ORDER BY s.MidiSoir";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Service service = new Service
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.Parse(reader.GetString(1)),
                                MidiSoir = reader.GetString(2),
                                MenuId = reader.GetInt32(3),
                                Statut = reader.GetString(4)
                            };

                            services.Add(service);
                        }
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// Récupère tous les services futurs (à partir d'aujourd'hui) pour un restaurant donné.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste de tous les services futurs du restaurant.</returns>
        public List<Service> GetAllServices(int restaurantId)
        {
            List<Service> services = new List<Service>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT s.Id, s.Date, s.MidiSoir, s.MenuId, s.Statut
                    FROM Services s
                    INNER JOIN Menus m ON s.MenuId = m.Id
                    WHERE DATE(s.Date) >= DATE('now')
                    AND m.RestaurantId = @RestaurantId
                    ORDER BY s.Date, s.MidiSoir";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RestaurantId", restaurantId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Service service = new Service
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.Parse(reader.GetString(1)),
                                MidiSoir = reader.GetString(2),
                                MenuId = reader.GetInt32(3),
                                Statut = reader.GetString(4)
                            };

                            services.Add(service);
                        }
                    }
                }
            }

            return services;
        }
    }
}