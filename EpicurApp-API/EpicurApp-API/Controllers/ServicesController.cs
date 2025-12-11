using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
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
        private readonly IAllergeneDetectionService _allergeneDetectionService;

        /// <summary>
        /// Constructeur : injection des DAO et services.
        /// </summary>
        public ServicesController(
            ServiceDAO serviceDAO, 
            ReservationDAO reservationDAO,
            IAllergeneDetectionService allergeneDetectionService)
        {
            _serviceDAO = serviceDAO;
            _reservationDAO = reservationDAO;
            _allergeneDetectionService = allergeneDetectionService;
        }

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
                return StatusCode(500, $"Erreur lors de la récupération des services: {ex.Message}");
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
                return StatusCode(500, $"Erreur lors de la récupération des services: {ex.Message}");
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
                    return BadRequest("Le service ne peut pas être null.");
                }

                int? restaurantId = GetRestaurantIdFromHeader();

                if (!restaurantId.HasValue)
                {
                    return BadRequest("Header X-Restaurant-Id requis.");
                }

                _serviceDAO.AjouterService(service, restaurantId.Value);
                return CreatedAtAction(nameof(GetAllServices), new { id = service.Id }, service);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout du service: {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une nouvelle réservation avec vérification des allergènes.
        /// Retourne un conflit 409 si des allergènes sont détectés (sauf si forcé).
        /// </summary>
        /// <param name="request">La requête de réservation.</param>
        /// <returns>La réservation créée ou les conflits détectés.</returns>
        [HttpPost("Reservation")]
        public IActionResult AjouterReservation([FromBody] ReservationRequest request,[FromQuery] bool force = false)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("La requête de réservation ne peut pas être null.");
                }

                bool estForce = force || request.ForceReservation;
                // Récupérer le service pour obtenir le MenuId
                var services = _serviceDAO.GetAllServices(GetRestaurantIdFromHeader() ?? 0);
                var service = services.FirstOrDefault(s => s.Id == request.ServiceId);
                
                if (service == null)
                {
                    return NotFound($"Service avec l'ID {request.ServiceId} introuvable.");
                }

                // Détecter les conflits d'allergènes
                List<ConflitAllergene> conflits = _allergeneDetectionService.DetecterConflits(request.ClientId, service.MenuId);

                if (conflits.Count > 0 && !estForce)
                {
                    // Retourner les conflits avec code 409
                    var response = new ValidationReservationResponse
                    {
                        EstValide = false,
                        ADesConflits = true,
                        Conflits = conflits,
                        Message = "Des conflits d'allergènes ont été détectés."
                    };
                    return Conflict(response);
                }

                string noteFinale = request.NoteOverride;
                if (conflits.Count > 0 && estForce && string.IsNullOrWhiteSpace(noteFinale))
                {
                    noteFinale = "Forcé par le restaurateur (Alerte allergie ignorée)";
                }

                // Créer la réservation
                var reservation = new Reservation
                {
                    ServiceId = request.ServiceId,
                    ClientId = request.ClientId,
                    NbCouverts = request.NbCouverts
                };

                _reservationDAO.AjouterReservation(reservation);

                // Retourner la réponse avec les infos de forçage si applicable
                var successResponse = new ValidationReservationResponse
                {
                    EstValide = true,
                    ADesConflits = conflits.Count > 0,
                    Conflits = conflits,
                    EstForcee = estForce,
                    NoteOverride = noteFinale,
                    ReservationId = reservation.Id,
                    Message = conflits.Count > 0 ? $"Réservation créée avec avertissement : {noteFinale}" : "Réservation créée avec succès."
                };

                return CreatedAtAction(nameof(GetReservationsParService), 
                    new { serviceId = reservation.ServiceId }, successResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout de la réservation: {ex.Message}");
            }
        }

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