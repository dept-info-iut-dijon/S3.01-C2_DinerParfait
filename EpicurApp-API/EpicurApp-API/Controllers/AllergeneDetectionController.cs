using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Contrôleur pour la détection des conflits d'allergènes.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AllergeneDetectionController : ControllerBase
    {
        private readonly IAllergeneDetectionService _detectionService;

        /// <summary>
        /// Constructeur du contrôleur.
        /// </summary>
        /// <param name="detectionService">Service de détection d'allergènes.</param>
        public AllergeneDetectionController(IAllergeneDetectionService detectionService)
        {
            _detectionService = detectionService;
        }

        /// <summary>
        /// Détecte les conflits d'allergènes entre un client et un menu.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des conflits détectés.</returns>
        [HttpGet("detecter/{clientId}/{menuId}")]
        public ActionResult<List<ConflitAllergene>> DetecterConflits(int clientId, int menuId)
        {
            try
            {
                List<ConflitAllergene> conflits = _detectionService.DetecterConflits(clientId, menuId);
                return Ok(conflits);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la détection des conflits : {ex.Message}");
            }
        }

        /// <summary>
        /// Détecte les conflits d'allergènes pour plusieurs clients sur un menu.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <param name="clientIds">Liste des identifiants des clients.</param>
        /// <returns>Liste des conflits détectés.</returns>
        [HttpPost("detecter-multiple/{menuId}")]
        public ActionResult<List<ConflitAllergene>> DetecterConflitsPourPlusieursClients(
            int menuId, 
            [FromBody] List<int> clientIds)
        {
            try
            {
                if (clientIds == null || clientIds.Count == 0)
                {
                    return BadRequest("La liste des clients ne peut pas être vide.");
                }

                List<ConflitAllergene> conflits = _detectionService
                    .DetecterConflitsPourPlusieursClients(clientIds, menuId);
                return Ok(conflits);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la détection des conflits : {ex.Message}");
            }
        }

        /// <summary>
        /// Valide une réservation en vérifiant les conflits d'allergènes.
        /// Permet de forcer la réservation avec une note explicative.
        /// </summary>
        /// <param name="request">Requête de réservation.</param>
        /// <returns>Réponse de validation avec les éventuels conflits.</returns>
        [HttpPost("valider-reservation")]
        public ActionResult<ValidationReservationResponse> ValiderReservation(
            [FromBody] ReservationRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("La requête de réservation est invalide.");
                }

                ValidationReservationResponse response = _detectionService.ValiderReservation(request);
                
                // Retourner un code approprié selon le résultat
                if (response.EstValide)
                {
                    return Ok(response);
                }
                else if (response.ADesConflits && !response.EstForcee)
                {
                    // Conflits détectés, retourner 409 Conflict
                    return Conflict(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la validation : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère la liste des allergènes présents dans un menu.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des allergènes du menu.</returns>
        [HttpGet("menu/{menuId}/allergenes")]
        public ActionResult<List<Allergene>> GetAllergenesParMenu(int menuId)
        {
            try
            {
                List<Allergene> allergenes = _detectionService.GetAllergenesParMenu(menuId);
                return Ok(allergenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des allergènes : {ex.Message}");
            }
        }
    }
}