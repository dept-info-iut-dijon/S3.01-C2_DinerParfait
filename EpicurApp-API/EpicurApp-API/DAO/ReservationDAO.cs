using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des réservations.
    /// </summary>
    public class ReservationDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de ReservationDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de données.</param>
        public ReservationDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Ajoute une nouvelle réservation dans la base de données.
        /// </summary>
        /// <param name="reservation">La réservation à ajouter.</param>
        public void AjouterReservation(Reservation reservation)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO Reservations 
                    (ServiceId, ClientId, NbCouverts) 
                    VALUES (@ServiceId, @ClientId, @NbCouverts);
                    SELECT last_insert_rowid();";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceId", reservation.ServiceId);
                    command.Parameters.AddWithValue("@ClientId", reservation.ClientId);
                    command.Parameters.AddWithValue("@NbCouverts", reservation.NbCouverts);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        reservation.Id = Convert.ToInt32(result);
                    }
                }
            }
        }

        /// <summary>
        /// Récupère toutes les réservations pour un service donné avec les informations du client.
        /// </summary>
        /// <param name="serviceId">Identifiant du service.</param>
        /// <returns>Liste des réservations avec nom et prénom du client.</returns>
        public List<Reservation> GetReservationsParService(int serviceId)
        {
            List<Reservation> reservations = new List<Reservation>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT r.Id, r.ServiceId, r.ClientId, r.NbCouverts, c.Nom, c.Prenom 
                    FROM Reservations r
                    INNER JOIN Clients c ON r.ClientId = c.Id
                    WHERE r.ServiceId = @ServiceId
                    ORDER BY c.Nom, c.Prenom";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ServiceId", serviceId);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reservation reservation = new Reservation
                            {
                                Id = reader.GetInt32(0),
                                ServiceId = reader.GetInt32(1),
                                ClientId = reader.GetInt32(2),
                                NbCouverts = reader.GetInt32(3),
                                NomClient = reader.GetString(4),
                                PrenomClient = reader.GetString(5)
                            };

                            reservations.Add(reservation);
                        }
                    }
                }
            }

            return reservations;
        }
    }
}