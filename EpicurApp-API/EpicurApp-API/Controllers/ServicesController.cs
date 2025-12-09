using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;
using EpicurApp_API.DAO;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour gérer les services et les réservations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ServiceDAO _serviceDAO;
        private readonly ReservationDAO _reservationDAO;

        /// <summary>
        /// Constructeur : injection des DAO.
        /// </summary>
        /// <param name="serviceDAO">DAO pour les services.</param>
        /// <param name="reservationDAO">DAO pour les réservations.</param>
        public ServicesController(ServiceDAO serviceDAO, ReservationDAO reservationDAO)
        {
            _serviceDAO = serviceDAO;
            _reservationDAO = reservationDAO;
        }

        /// <summary>
        /// Récupère tous les services futurs.
        /// </summary>
        /// <returns>Liste des services futurs.</returns>
        [HttpGet]
        public IActionResult GetAllServices()
        {
            try
            {
                List<Service> services = _serviceDAO.GetAllServices();
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des services: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère les services pour une date donnée.
        /// </summary>
        /// <param name="date">Date au format yyyy-MM-dd.</param>
        /// <returns>Liste des services pour cette date.</returns>
        [HttpGet("ByDate/{date}")]
        public IActionResult GetServicesParDate(string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest("Format de date invalide. Utilisez yyyy-MM-dd.");
                }

                List<Service> services = _serviceDAO.GetServicesParDate(parsedDate);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des services: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute un nouveau service.
        /// </summary>
        /// <param name="service">Le service à ajouter.</param>
        /// <returns>Le service créé avec son ID.</returns>
        [HttpPost]
        public IActionResult AjouterService([FromBody] Service service)
        {
            try
            {
                if (service == null)
                {
                    return BadRequest("Le service ne peut pas être null.");
                }

                _serviceDAO.AjouterService(service);
                return CreatedAtAction(nameof(GetAllServices), new { id = service.Id }, service);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout du service: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une nouvelle réservation.
        /// </summary>
        /// <param name="reservation">La réservation à ajouter.</param>
        /// <returns>La réservation créée avec son ID.</returns>
        [HttpPost("Reservation")]
        public IActionResult AjouterReservation([FromBody] Reservation reservation)
        {
            try
            {
                if (reservation == null)
                {
                    return BadRequest("La réservation ne peut pas être null.");
                }

                _reservationDAO.AjouterReservation(reservation);
                return CreatedAtAction(nameof(GetReservationsParService), new { serviceId = reservation.ServiceId }, reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout de la réservation: {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère toutes les réservations pour un service donné.
        /// </summary>
        /// <param name="serviceId">Identifiant du service.</param>
        /// <returns>Liste des réservations avec nom et prénom des clients.</returns>
        [HttpGet("{serviceId}/Reservations")]
        public IActionResult GetReservationsParService(int serviceId)
        {
            try
            {
                List<Reservation> reservations = _reservationDAO.GetReservationsParService(serviceId);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des réservations: {ex.Message}");
            }
        }
    }
}