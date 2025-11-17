using EpicurApp_API.DAO;
using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des allergènes
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AllergenesController : ControllerBase
    {
        private readonly AllergeneDAO _allergeneDAO;

        public AllergenesController(AllergeneDAO allergeneDAO)
        {
            _allergeneDAO = allergeneDAO;
        }

        /// <summary>
        /// Récupère tous les allergènes
        /// </summary>
        /// <returns>Liste des allergènes</returns>
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

        /// <summary>
        /// Ajoute un nouvel allergène
        /// </summary>
        /// <param name="allergene">L'allergène à ajouter</param>
        /// <returns>L'allergène créé</returns>
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
