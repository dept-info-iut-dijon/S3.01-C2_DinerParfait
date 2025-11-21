using System.Runtime.InteropServices;
using EpicurApp_API.DAO;
using EpicurAPP_Partage.Models;
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
        private readonly IdeePlatDAO _ideePlatDAO;

        public IdeePlatController(IdeePlatDAO ideePlatDAO)
        {
            _ideePlatDAO = ideePlatDAO;
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var idees = _ideePlatDAO.GetAll();
            return Ok(idees);
        }

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        [HttpPost]
        public IActionResult Ajouter([FromBody] IdeePlat idee)
        {
            if (string.IsNullOrWhiteSpace(idee.Titre))
            {
                return BadRequest("Le titre est obligatoire");
            }

            _ideePlatDAO.Ajouter(idee);
            return Ok(idee);
        }

        /// <summary>
        /// Modifie une idée de plat.
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Modifier(int id, [FromBody] IdeePlat idee)
        {
            idee.Id = id;
            _ideePlatDAO.Modifier(idee);
            return Ok(idee);
        }

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Supprimer(int id)
        {
            _ideePlatDAO.Supprimer(id);
            return Ok();
        }
    }
}