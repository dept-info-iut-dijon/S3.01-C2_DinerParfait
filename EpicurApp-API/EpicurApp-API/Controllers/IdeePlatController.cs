using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour la gestion des idées de plats.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class IdeePlatController : ControllerBase
    {
        /// <summary>
        /// IService pour gérer les idées de plats.
        /// </summary>
        private readonly IIdeePlatService _ideePlatService;
        private readonly IIdeePlatDAO _ideePlatDAO;

        /// <summary>
        /// Constructeur : injection du service et du DAO des idées de plats.
        /// </summary>
        /// <param name="ideePlatService">idée des plats a servir</param>
        /// <param name="ideePlatDAO">DAO des idées de plats</param>
        public IdeePlatController(IIdeePlatService ideePlatService, IIdeePlatDAO ideePlatDAO)
        {
            _ideePlatService = ideePlatService;
            _ideePlatDAO = ideePlatDAO;
        }

        /// <summary>
        /// Helper pour récupérer le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <returns>RestaurantId ou null si non fourni.</returns>
        private int? GetRestaurantIdFromHeader()
        {
            Microsoft.Extensions.Primitives.StringValues restaurantIdValue;

            if (Request.Headers.TryGetValue("X-Restaurant-Id", out restaurantIdValue))
            {
                int restaurantId;
                if (int.TryParse(restaurantIdValue, out restaurantId))
                {
                    return restaurantId;
                }
            }
            return null;
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// Utilise le header X-Restaurant-Id pour filtrer par restaurant.
        /// </summary>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Récupération du RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();

                List<IdeePlat> idees;

                if (restaurantId.HasValue)
                {
                    // Filtrage par restaurant
                    idees = _ideePlatDAO.GetAllByRestaurantId(restaurantId.Value);
                }
                else
                {
                    // Sans filtre (pour compatibilité)
                    idees = _ideePlatService.ObtenirToutesLesIdees();
                }

                return Ok(idees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des idées : {ex.Message}");
            }
        }

        /// <summary>
        /// Récupère une idée de plat par son ID.
        /// Vérifie que l'idée appartient au restaurant de l'utilisateur via X-Restaurant-Id.
        /// </summary>
        /// <param name="id">ID de l'idée à récupérer</param>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                IdeePlat? idee = _ideePlatService.ObtenirIdeeParId(id);
                if (idee == null)
                {
                    return NotFound($"Aucune idée de plat trouvée avec l'ID {id}");
                }

                // Vérifier que l'idée appartient au restaurant de l'utilisateur
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && idee.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden
                }

                return Ok(idee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération de l'idée : {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// Assigne automatiquement le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <exception cref="Exception">En cas d'erreur lors de l'ajout</exception>
        [HttpPost]
        public IActionResult Ajouter([FromBody] IdeePlat idee)
        {
            try
            {
                // Assigner automatiquement le RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue)
                {
                    idee.RestaurantId = restaurantId.Value;
                }

                _ideePlatService.AjouterIdee(idee);
                return Ok(idee);
            }
            catch (EpicurAPP_Partage.Exceptions.InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout : {ex.Message}");
            }
        }

        /// <summary>
        /// Modifie une idée de plat.
        /// Vérifie que l'idée appartient au restaurant de l'utilisateur.
        /// </summary>
        /// <param name="id">Identifiant de l'idée à modifier</param>
        /// <exception cref="Exception">En cas d'erreur lors de la modification</exception>
        [HttpPut("{id}")]
        public IActionResult Modifier(int id, [FromBody] IdeePlat idee)
        {
            try
            {
                // Vérifier que l'idée appartient au restaurant de l'utilisateur
                IdeePlat? existingIdee = _ideePlatService.ObtenirIdeeParId(id);
                if (existingIdee == null)
                {
                    return NotFound($"Aucune idée de plat trouvée avec l'ID {id}");
                }

                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && existingIdee.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden
                }

                // S'assurer que le RestaurantId ne change pas
                idee.Id = id;
                idee.RestaurantId = existingIdee.RestaurantId;

                _ideePlatService.ModifierIdee(idee);
                return Ok(idee);
            }
            catch (EpicurAPP_Partage.Exceptions.InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la modification : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        /// <param name="id">Identifiant de l'idée à supprimer</param>
        /// <exception cref="Exception">En cas d'erreur lors de la suppression</exception>
        [HttpDelete("{id}")]
        public IActionResult Supprimer(int id)
        {
            try
            {
                _ideePlatService.SupprimerIdee(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression : {ex.Message}");
            }
        }
    }
}
