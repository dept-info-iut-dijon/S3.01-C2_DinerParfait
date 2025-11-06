using EpicurApp_API.DAO;
using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlatsController : Controller
    {
        private readonly IPlatDAO _platDAO;

        public PlatsController(IPlatDAO platDAO)
        {
            _platDAO = platDAO;
        }

        // GET: plats
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

        // GET: plats
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

        // GET: plats/categorie/{categorie}
        [HttpGet("categorie/{categorie}")]
        public ActionResult<IEnumerable<Plat>> GetPlatsByCategorie(string categorie)
        {
            try
            {
                List<Plat> plats = _platDAO.GetAll()
                    .Where(p => p.Categorie.Equals(categorie, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return Ok(plats);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erreur interne du serveur : {e.Message}");
            }
        }

        // POST: plats
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