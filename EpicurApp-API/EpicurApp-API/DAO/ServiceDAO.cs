using EpicurApp_API.Configuration;
using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

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
        /// <param name="restaurantId">Identifiant du restaurant pour valider que le menu appartient bien � ce restaurant.</param>
        public void AjouterService(Service service, int restaurantId)
        {
            
            if (service.Date.HasValue)
            {
                double heuresDifference = (service.Date.Value - DateTime.Now).TotalHours;

                if (service.Date.Value.Date < DateTime.Now.Date)
                {
                    throw new Exception($"Tentative de création dans le passé : {service.Date.Value}");
                }
            }


            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (service.Date.HasValue)
                        {
                            string checkQuery = @"SELECT COUNT(*) FROM Services 
                                          WHERE date(Date) = date(@Date) 
                                          AND MidiSoir = @MidiSoir";

                            using (SqliteCommand cmdCheck = new SqliteCommand(checkQuery, connection, transaction))
                            {
                                string dateParam = service.Date.Value.ToString("yyyy-MM-dd HH:mm:ss");
                                cmdCheck.Parameters.AddWithValue("@Date", dateParam);
                                cmdCheck.Parameters.AddWithValue("@MidiSoir", service.MidiSoir);

                                Debug.WriteLine($"[DEBUG] Check Doublon SQL avec Date='{dateParam}' et MidiSoir='{service.MidiSoir}'");

                                long existe = (long)(cmdCheck.ExecuteScalar() ?? 0);
                                Debug.WriteLine($"[DEBUG] Résultat Check Doublon : {existe}");

                                if (existe > 0)
                                {
                                    throw new Exception($"[DEBUG BLOCK] Doublon détecté en base pour {dateParam} - {service.MidiSoir}");
                                }
                            }
                        }

                        string insertQuery = @"INSERT INTO Services (Date, MidiSoir, MenuId, Statut)
                                       VALUES (@Date, @MidiSoir, @MenuId, @Statut);
                                       SELECT last_insert_rowid();";

                        using (SqliteCommand command = new SqliteCommand(insertQuery, connection, transaction))
                        {
                            object dateValue = service.Date.HasValue ? service.Date.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value;


                            command.Parameters.AddWithValue("@Date", dateValue);
                            command.Parameters.AddWithValue("@MidiSoir", service.MidiSoir);
                            command.Parameters.AddWithValue("@MenuId", service.MenuId);
                            command.Parameters.AddWithValue("@Statut", service.Statut);

                            var result = command.ExecuteScalar();

                            if (result != null) service.Id = Convert.ToInt32(result);
                        }

                        if (service.Date.HasValue)
                        {
                            string updateMenu = "UPDATE Menus SET Date = @Date WHERE Id = @MenuId";

                            using (SqliteCommand cmdMenu = new SqliteCommand(updateMenu, connection, transaction))
                            {
                                cmdMenu.Parameters.AddWithValue("@Date", service.Date.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmdMenu.Parameters.AddWithValue("@MenuId", service.MenuId);
                                int rows = cmdMenu.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void UpdateMenuService(int serviceId, int nouveauMenuId)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Date FROM Services WHERE Id = @Id";
                DateTime? dateReelle = null;

                using (SqliteCommand selectCmd = new SqliteCommand(selectQuery, connection))
                {
                    selectCmd.Parameters.AddWithValue("@Id", serviceId);
                    var result = selectCmd.ExecuteScalar();

                    if (result == null) throw new KeyNotFoundException($"Service {serviceId} introuvable.");
                    if (result != DBNull.Value && result.ToString() != null) dateReelle = DateTime.Parse(result.ToString()!);
                }

                Service serviceActuel = new Service { Date = dateReelle };

                if (serviceActuel.EstVerrouille)
                {
                    throw new InvalidOperationException($"INTERDIT : Ce service est verrouillé (Reste moins de 48h ou passé).");
                }

                string updateQuery = "UPDATE Services SET MenuId = @MenuId WHERE Id = @Id";
                using (SqliteCommand updateCmd = new SqliteCommand(updateQuery, connection))
                {
                    updateCmd.Parameters.AddWithValue("@MenuId", nouveauMenuId);
                    updateCmd.Parameters.AddWithValue("@Id", serviceId);
                    updateCmd.ExecuteNonQuery();
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