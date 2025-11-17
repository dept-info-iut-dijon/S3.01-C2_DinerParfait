using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des plats
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlatsController : Controller
    {
        private readonly IPlatDAO _platDAO;

        public PlatsController(IPlatDAO platDAO)
        {
            _platDAO = platDAO;
        }

        /// <summary>
        /// Récupère tous les plats
        /// </summary>
        /// <returns>Liste de tous les plats</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Plat>> GetAllPlats()
        {
            try
            {
                List<Plat> plats = _platDAO.GetAll();

                if (plats == null || !plats.Any())
                {
                    return Ok(new List<Plat>());
                }

                return Ok(plats);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        /// <summary>
        /// Récupère un plat par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du plat</param>
        /// <returns>Le plat correspondant</returns>
        [HttpGet("{id}")]
        public ActionResult<Plat> GetPlatById(int id)
        {
            try
            {
                Plat plat = _platDAO.GetById(id);
                if (plat == null)
                {
                    return NotFound($"Aucun plat trouvé avec l'ID {id}");
                }
                return Ok(plat);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        /// <summary>
        /// Récupère les plats par catégorie
        /// </summary>
        /// <param name="categorie">Nom de la catégorie</param>
        /// <returns>Liste des plats de la catégorie</returns>
        [HttpGet("categorie/{categorie}")]
        public ActionResult<IEnumerable<Plat>> GetPlatsByCategorie(string categorie)
        {
            try
            {
                List<Plat> plats = _platDAO.GetAll()
                    .Where(p => p.Categorie.ToString().Equals(categorie, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return Ok(plats);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        /// <summary>
        /// Crée un nouveau plat
        /// </summary>
        /// <param name="plat">Les données du plat à créer</param>
        /// <returns>Le plat créé</returns>
        [HttpPost]
        public ActionResult<Plat> CreatePlat([FromBody] Plat plat)
        {
            if (plat == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _platDAO.Add(plat);
                return CreatedAtAction(nameof(GetPlatById), new { id = plat.Id }, plat);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erreur lors de la création : {e.Message}");
            }
        }
    }
}
