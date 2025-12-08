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
        /// Récupère tous les services pour une date donnée.
        /// </summary>
        /// <param name="date">Date pour laquelle récupérer les services.</param>
        /// <returns>Liste des services trouvés.</returns>
        public List<Service> GetServicesParDate(DateTime date)
        {
            List<Service> services = new List<Service>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Date, MidiSoir, MenuId, Statut 
                    FROM Services 
                    WHERE DATE(Date) = DATE(@Date)
                    ORDER BY MidiSoir";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));

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
        /// Récupère tous les services futurs (à partir d'aujourd'hui).
        /// </summary>
        /// <returns>Liste de tous les services futurs.</returns>
        public List<Service> GetAllServices()
        {
            List<Service> services = new List<Service>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT Id, Date, MidiSoir, MenuId, Statut 
                    FROM Services 
                    WHERE DATE(Date) >= DATE('now')
                    ORDER BY Date, MidiSoir";

                using (SqliteCommand command = new SqliteCommand(query, connection))
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

            return services;
        }
    }
}