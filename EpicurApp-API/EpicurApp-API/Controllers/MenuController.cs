using EpicurAPP_Partage.Models;
using EpicurAPP_Partage.Exceptions;
using Microsoft.AspNetCore.Mvc;
using EpicurAppLogic.Interfaces;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour gérer les menus.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        /// <summary>
        /// Service permettant de gérer les opérations sur les menus.
        /// </summary>
        private readonly IMenuService _menuService;
        private readonly IMenuDAO _menuDAO;

        /// <summary>
        /// Constructeur : injection du service menu et du DAO.
        /// </summary>
        /// <param name="menuService">service du menu</param>
        /// <param name="menuDAO">DAO du menu</param>
        public MenuController(IMenuService menuService, IMenuDAO menuDAO)
        {
            _menuService = menuService;
            _menuDAO = menuDAO;
        }

        /// <summary>
        /// Helper pour récupérer le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <returns>RestaurantId ou null si non fourni.</returns>
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
        /// Méthode GET pour récupérer tous les menus.
        /// Utilise le header X-Restaurant-Id pour filtrer par restaurant.
        /// </summary>
        /// <exception cref="Exception">Lance une exception en cas d'erreur lors de la récupération des menus.</exception>
        /// <returns>Promesse d'une liste de menu</returns>
        [HttpGet]
        public ActionResult<List<Menu>> GetAll()
        {
            try
            {
                // Récupération du RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();

                List<Menu> menus;

                if (restaurantId.HasValue)
                {
                    // Filtrage par restaurant
                    menus = _menuDAO.GetAllByRestaurantId(restaurantId.Value);
                }
                else
                {
                    // Sans filtre (pour compatibilité)
                    menus = _menuService.GetAll();
                }

                return Ok(menus);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la récupération des menus: {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode GET pour récupérer un menu par son ID.
        /// Vérifie que le menu appartient au restaurant de l'utilisateur via X-Restaurant-Id.
        /// </summary>
        /// <param name="id">ID du menu cible</param>
        /// <returns>Le menu avec l'ID correspondant</returns>
        [HttpGet("{id}")]
        public ActionResult<Menu> GetById(int id)
        {
            try
            {
                Menu? menu = _menuService.GetById(id);
                if (menu == null)
                    return NotFound($"Menu avec l'ID {id} introuvable");

                // Vérifier que le menu appartient au restaurant de l'utilisateur
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && menu.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden - le menu existe mais n'appartient pas à ce restaurant
                }

                return Ok(menu);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération du menu : {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode GET pour générer la liste de courses d'un menu.
        /// </summary>
        /// <param name="id">ID du menu</param>
        /// <exception cref="EntityNotFoundException">Lance une exception si le menu n'est pas trouvé.</exception>
        /// <exception cref="Exception">Lance une exception en cas d'erreur lors de la génération de la liste de courses.</exception>
        /// <returns>Promesse d'une liste d'elements pour la liste de course</returns>
        [HttpGet("{id}/courses")]
        public ActionResult<List<ElementListeCourse>> GetListeCourses(int id)
        {
            try
            {
                List<ElementListeCourse> liste = _menuService.GenererListeCourses(id);
                return Ok(liste);
            }
            catch (EntityNotFoundException)
            {
                return NotFound("Menu introuvable");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Méthode POST pour ajouter un nouveau menu.
        /// Assigne automatiquement le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <param name="menu">Menu a ajouter</param>
        /// <exception cref="ValidationException">Lance une exception en cas de validation échouée.</exception>
        /// <returns>Code201 sinon une exeption</returns>
        [HttpPost]
        public ActionResult Add([FromBody] Menu menu)
        {
            try
            {
                // Assigner automatiquement le RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue)
                {
                    menu.RestaurantId = restaurantId.Value;
                }

                _menuService.AjouterMenu(menu);
                return CreatedAtAction(nameof(GetById), new { id = menu.Id }, menu);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'ajout du menu : {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode PUT pour mettre à jour un menu existant.
        /// Vérifie que le menu appartient au restaurant de l'utilisateur.
        /// </summary>
        /// <param name="id">ID du menu</param>
        /// <param name="menu">Menu mis à jour</param>
        /// <exception cref="ValidationException">Lance une exception en cas de validation échouée.</exception>
        /// <returns>code 204 NoContent sinon exeption</returns>
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Menu menu)
        {
            if (id != menu.Id)
                return BadRequest("L'ID de l'URL ne correspond pas à l'ID du corps de la requête.");

            try
            {
                // Vérifier que le menu appartient au restaurant de l'utilisateur
                Menu? existingMenu = _menuService.GetById(id);
                if (existingMenu == null)
                    return NotFound($"Menu avec l'ID {id} introuvable");

                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && existingMenu.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden
                }

                // S'assurer que le RestaurantId ne change pas
                menu.RestaurantId = existingMenu.RestaurantId;

                _menuService.MettreAJourMenu(menu);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la mise à jour du menu : {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode DELETE pour supprimer un menu par son ID.
        /// </summary>
        /// <param name="id">id du menu a supprimer</param>
        /// <exception cref="InvalidFieldException">Lance une exception en cas de champ invalide.</exception>
        /// <exception cref="Exception">Lance une exception en cas d'erreur lors de la suppression du menu.</exception>
        /// <returns>code 201 sinon exeption</returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                _menuService.SupprimerMenu(id);
                return NoContent();
            }
            catch (InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la suppression du menu: {ex.Message}");
            }
        }
    }
}
