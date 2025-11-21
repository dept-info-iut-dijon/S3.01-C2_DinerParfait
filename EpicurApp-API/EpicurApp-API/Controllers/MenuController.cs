using EpicurAPP_Partage.Models;
using EpicurAPP_Partage.Exceptions;
using Microsoft.AspNetCore.Mvc;
using EpicurAppLogic.Interfaces;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // GET: api/Menu
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

        // GET: api/Menu/5
        [HttpGet("{id}")]
        public ActionResult<Menu> GetById(int id)
        {
            Menu? menu = _menuService.GetById(id);
            if (menu == null) return NotFound();
            return Ok(menu);
        }

        // GET: api/Menu/5/courses
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

        // POST: api/Menu
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

        // PUT: api/Menu/5
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

        // DELETE: api/Menu/5
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
