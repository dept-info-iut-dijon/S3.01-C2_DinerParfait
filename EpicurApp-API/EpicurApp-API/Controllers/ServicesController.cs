using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;
using EpicurApp_API.DAO;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour g�rer les services et les r�servations.
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
        /// <param name="reservationDAO">DAO pour les r�servations.</param>
        public ServicesController(ServiceDAO serviceDAO, ReservationDAO reservationDAO)
        {
            _serviceDAO = serviceDAO;
            _reservationDAO = reservationDAO;
        }

        /// <summary>
        /// R�cup�re l'ID du restaurant depuis le header X-Restaurant-Id.
        /// </summary>
        /// <returns>L'ID du restaurant ou null si non trouv�.</returns>
        private int? GetRestaurantIdFromHeader()
        {
            if (Request.Headers.TryGetValue("X-Restaurant-Id", out var restaurantIdValue))
            {
                if (int.TryParse(restaurantIdValue, out int restaurantId))
                {
                    return restaurantId;
                }
            }
            return null;
        }

        /// <summary>
        /// R�cup�re tous les services futurs du restaurant.
        /// </summary>
        /// <returns>Liste des services futurs du restaurant.</returns>
        [HttpGet]
        public IActionResult GetAllServices()
        {
            try
            {
                int? restaurantId = GetRestaurantIdFromHeader();

                if (!restaurantId.HasValue)
                {
                    return BadRequest("Header X-Restaurant-Id requis.");
                }

                List<Service> services = _serviceDAO.GetAllServices(restaurantId.Value);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la r�cup�ration des services: {ex.Message}");
            }
        }

        /// <summary>
        /// R�cup�re les services pour une date donn�e et le restaurant.
        /// </summary>
        /// <param name="date">Date au format yyyy-MM-dd.</param>
        /// <returns>Liste des services pour cette date et ce restaurant.</returns>
        [HttpGet("ByDate/{date}")]
        public IActionResult GetServicesParDate(string date)
        {
            try
            {
                int? restaurantId = GetRestaurantIdFromHeader();

                if (!restaurantId.HasValue)
                {
                    return BadRequest("Header X-Restaurant-Id requis.");
                }

                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest("Format de date invalide. Utilisez yyyy-MM-dd.");
                }

                List<Service> services = _serviceDAO.GetServicesParDate(parsedDate, restaurantId.Value);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la r�cup�ration des services: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute un nouveau service.
        /// </summary>
        /// <param name="service">Le service � ajouter.</param>
        /// <returns>Le service cr�� avec son ID.</returns>
        [HttpPost]
        public IActionResult AjouterService([FromBody] Service service)
        {
            try
            {
                if (service == null)
                {
                    return BadRequest("Le service ne peut pas �tre null.");
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
        /// Ajoute une nouvelle r�servation.
        /// </summary>
        /// <param name="reservation">La r�servation � ajouter.</param>
        /// <returns>La r�servation cr��e avec son ID.</returns>
        [HttpPost("Reservation")]
        public IActionResult AjouterReservation([FromBody] Reservation reservation)
        {
            try
            {
                if (reservation == null)
                {
                    return BadRequest("La r�servation ne peut pas �tre null.");
                }

                _reservationDAO.AjouterReservation(reservation);
                return CreatedAtAction(nameof(GetReservationsParService), new { serviceId = reservation.ServiceId }, reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout de la r�servation: {ex.Message}");
            }
        }

        /// <summary>
        /// R�cup�re toutes les r�servations pour un service donn�.
        /// </summary>
        /// <param name="serviceId">Identifiant du service.</param>
        /// <returns>Liste des r�servations avec nom et pr�nom des clients.</returns>
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
                return StatusCode(500, $"Erreur lors de la r�cup�ration des r�servations: {ex.Message}");
            }
        }
    }
}