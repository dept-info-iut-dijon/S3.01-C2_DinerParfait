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
        private readonly IIdeePlatService _ideePlatService;

        public IdeePlatController(IIdeePlatService ideePlatService)
        {
            _ideePlatService = ideePlatService;
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var idees = _ideePlatService.ObtenirToutesLesIdees();
                return Ok(idees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des idées : {ex.Message}");
            }
        }

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        [HttpPost]
        public IActionResult Ajouter([FromBody] IdeePlat idee)
        {
            try
            {
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
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Modifier(int id, [FromBody] IdeePlat idee)
        {
            try
            {
                idee.Id = id;
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