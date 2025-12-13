using EpicurAPP_Partage.Models;
using Microsoft.Data.Sqlite;
using EpicurApp_API.Configuration;

namespace EpicurApp_API.DAO
{
    /// <summary>
    /// DAO pour la gestion des r�servations.
    /// </summary>
    public class ReservationDAO
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise une nouvelle instance de ReservationDAO.
        /// </summary>
        /// <param name="databaseConfiguration">Configuration de la base de donn�es.</param>
        public ReservationDAO(DatabaseConfiguration databaseConfiguration)
        {
            _connectionString = databaseConfiguration.GetConnectionString();
        }

        /// <summary>
        /// Ajoute une nouvelle r�servation dans la base de donn�es.
        /// </summary>
        /// <param name="reservation">La r�servation � ajouter.</param>
        public void AjouterReservation(Reservation reservation)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // V�rifier que le service existe
                string checkServiceQuery = @"SELECT COUNT(*) FROM Services WHERE Id = @ServiceId";
                using (SqliteCommand checkServiceCommand = new SqliteCommand(checkServiceQuery, connection))
                {
                    checkServiceCommand.Parameters.AddWithValue("@ServiceId", reservation.ServiceId);

                    long serviceCount = (long)(checkServiceCommand.ExecuteScalar() ?? 0);
                    if (serviceCount == 0)
                    {
                        throw new InvalidOperationException($"Le service avec l'ID {reservation.ServiceId} n'existe pas.");
                    }
                }

                // V�rifier que le client existe
                string checkClientQuery = @"SELECT COUNT(*) FROM Clients WHERE Id = @ClientId";
                using (SqliteCommand checkClientCommand = new SqliteCommand(checkClientQuery, connection))
                {
                    checkClientCommand.Parameters.AddWithValue("@ClientId", reservation.ClientId);

                    long clientCount = (long)(checkClientCommand.ExecuteScalar() ?? 0);
                    if (clientCount == 0)
                    {
                        throw new InvalidOperationException($"Le client avec l'ID {reservation.ClientId} n'existe pas.");
                    }
                }

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
        /// R�cup�re toutes les r�servations pour un service donn� avec les informations du client.
        /// </summary>
        /// <param name="serviceId">Identifiant du service.</param>
        /// <returns>Liste des r�servations avec nom et pr�nom du client.</returns>
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