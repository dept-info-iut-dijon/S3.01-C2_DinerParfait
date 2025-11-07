using EpicurApp_API.DAO;
using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AllergenesController : ControllerBase
    {
        private AllergeneDAO _allergeneDAO;

        public AllergenesController(AllergeneDAO allergeneDAO)
        {
            _allergeneDAO = allergeneDAO;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                List<Allergene> allergenes = _allergeneDAO.GetAll();
                return Ok(allergenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AjouterAllergene([FromBody] Allergene allergene)
        {
            try
            {
                _allergeneDAO.AjouterAllergene(allergene);
                return StatusCode(201, allergene);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur: {ex.Message}");
            }
        }
    }
}