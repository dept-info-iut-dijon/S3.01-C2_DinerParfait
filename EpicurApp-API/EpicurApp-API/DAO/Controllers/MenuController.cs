using EpicurApp_API.Models;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EpicurApp_API.DAO.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Crée un nouveau menu.
        /// </summary>
        /// <param name="menu">Données du menu à créer.</param>
        /// <returns>Une réponse HTTP.</returns>
        [HttpPost]
        public IActionResult CreateMenu(Menu menu)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _menuService.AjouterMenu(menu);
                return StatusCode(StatusCodes.Status201Created, menu);
            }
            catch (InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la création du menu.");
            }
        }

        /// <summary>
        /// Met à jour un menu existant.
        /// </summary>
        /// <param name="id">Identifiant du menu.</param>
        /// <param name="menu">Données à jour du menu.</param>
        /// <returns>Une réponse HTTP appropriée.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateMenu(int id, Menu menu)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (menu == null)
                return BadRequest("Les données du menu sont manquantes.");

            if (menu.Id == 0)
            {
                menu.Id = id;
            }

            if (menu.Id != id)
                return BadRequest("L'identifiant du menu ne correspond pas.");

            try
            {
                _menuService.MettreAJourMenu(menu);
                return NoContent();
            }
            catch (InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la mise à jour du menu.");
            }
        }

        /// <summary>
        /// Récupère un menu par son Id.
        /// </summary>
        /// <param name="id">Identifiant du menu.</param>
        /// <returns>Le menu ou une erreur 404.</returns>
        [HttpGet("{id}")]
        public IActionResult GetMenuById(int id)
        {
            try
            {
                Menu menu = _menuService.GetById(id);
                if (menu == null)
                    return NotFound($"Menu avec Id {id} non trouvé.");

                return Ok(menu);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération du menu.");
            }
        }

        /// <summary>
        /// Récupère tous les menus.
        /// </summary>
        /// <returns>Liste des menus.</returns>
        [HttpGet("GetAll")]
        public IActionResult GetAllMenus()
        {
            try
            {
                List<Menu> menus = _menuService.GetAll();
                return Ok(menus);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des menus.");
            }
        }

        /// <summary>
        /// Récupère le dernier menu enregistré en brouillon.
        /// </summary>
        /// <returns>Le menu en brouillon ou une erreur 404.</returns>
        [HttpGet("Brouillon")]
        public IActionResult GetDernierBrouillon()
        {
            try
            {
                Menu? menu = _menuService.GetDernierBrouillon();
                if (menu == null)
                {
                    return NotFound("Aucun menu en brouillon disponible.");
                }

                return Ok(menu);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération du brouillon de menu.");
            }
        }

        /// <summary>
        /// Ajoute des plats existants à un menu.
        /// </summary>
        /// <param name="menuId">Id du menu.</param>
        /// <param name="platIds">Liste des Ids de plats à ajouter.</param>
        /// <returns>Réponse HTTP.</returns>
        [HttpPost("{menuId}/AddPlats")]
        public IActionResult AjouterPlatsAuMenu(int menuId, [FromBody] List<int> platIds)
        {
            if (platIds == null || platIds.Count == 0)
                return BadRequest("Au moins un plat doit être sélectionné.");

            try
            {
                _menuService.AjouterPlatsAuMenu(menuId, platIds);
                return Ok("Plats ajoutés au menu avec succès.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de l'ajout des plats au menu.");
            }
        }
    }
}



