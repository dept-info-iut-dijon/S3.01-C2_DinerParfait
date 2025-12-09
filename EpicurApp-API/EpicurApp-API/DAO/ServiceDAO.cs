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
        /// <param name="databaseConfiguration">Configuration de la base de donn�es.</param>
        public ServiceDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Ajoute un nouveau service dans la base de donn�es.
        /// </summary>
        /// <param name="service">Le service � ajouter.</param>
        public void AjouterService(Service service)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

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
        /// R�cup�re tous les services pour une date donn�e et un restaurant donn�.
        /// </summary>
        /// <param name="date">Date pour laquelle r�cup�rer les services.</param>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des services trouv�s.</returns>
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
        /// R�cup�re tous les services futurs (� partir d'aujourd'hui) pour un restaurant donn�.
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