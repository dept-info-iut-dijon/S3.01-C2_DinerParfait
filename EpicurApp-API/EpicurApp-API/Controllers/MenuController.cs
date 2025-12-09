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
        /// <summary>
        /// Constructeur : injection du service menu.
        /// </summary>
        /// <param name="menuService">service du menu</param>
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Méthode GET pour récupérer tous les menus.
        /// </summary>
        /// <exception cref="Exception">Lance une exception en cas d'erreur lors de la récupération des menus.</exception>
        /// <returns>Promesse d'une liste de menu</returns>
        [HttpGet]
        public ActionResult<List<Menu>> GetAll()
        {
            try
            {
                List<Menu> menus = _menuService.GetAll();
                return Ok(menus);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la récupération des menus: {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode GET pour récupérer un menu par son ID.
        /// </summary>
        /// <param name="id">ID de la personne cible</param>
        /// <returns>La personne avec l'ID correspondant</returns>
        [HttpGet("{id}")]
        public ActionResult<Menu> GetById(int id)
        {
            Menu? menu = _menuService.GetById(id);
            if (menu == null) return NotFound();
            return Ok(menu);
        }

        /// <summary>
        /// Méthode GET pour récupérer le dernier brouillon de menu.
        /// </summary>
        /// <returns>Le dernier menu en statut Brouillon ou NotFound</returns>
        [HttpGet("brouillon")]
        public ActionResult<Menu> GetBrouillon()
        {
            try
            {
                List<Menu> menus = _menuService.GetAll();
                Menu? brouillon = menus.FirstOrDefault(m => m.Statut == "Brouillon");
                if (brouillon == null) return NotFound();
                return Ok(brouillon);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la récupération du brouillon: {ex.Message}");
            }
        }

        /// <summary>
        /// Méthode GET pour générer la liste de courses d'un menu.
        /// </summary>
        /// <param name="id">ID du menu</param>
        /// <exception cref="EntityNotFoundException">Lance une exception si le menu n'est pas trouvé.</exception>
        /// <exception cref="Exception">Lance une exception en cas d'erreur lors de la génération de la liste de courses.</exception>
        /// <returns>Promesse d'une liste d'elements pour la liste de course</returns>
        [HttpGet("{id}/listecourses")]
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
        /// </summary>
        /// <param name="menu">Menu a ajouter</param>
        /// <exception cref="ValidationException">Lance une exception en cas de validation échouée.</exception>
        /// <returns>Code201 sinon une exeption</returns>
        [HttpPost] 
        public ActionResult Add([FromBody] Menu menu)
        {
            try
            {
                _menuService.AjouterMenu(menu);
                return CreatedAtAction(nameof(GetById), new { id = menu.Id }, menu);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Méthode PUT pour mettre à jour un menu existant.
        /// </summary>
        /// <param name="id">ID du menu</param>
        /// <param name="menu">nm du menu</param>
        /// <exception cref="ValidationException">Lance une exception en cas de validation échouée.</exception>
        /// <returns>code 201 sinon exeption</returns>
        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Menu menu)
        {
            if (id != menu.Id) return BadRequest("L'ID de l'URL ne correspond pas à l'ID du corps de la requête.");

            try
            {
                _menuService.MettreAJourMenu(menu);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
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
