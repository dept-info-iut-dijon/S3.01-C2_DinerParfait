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
        /// <summary>
        /// IService pour gérer les idées de plats.
        /// </summary>
        private readonly IIdeePlatService _ideePlatService;

        /// <summary>
        /// Constructeur : injection du service des idées de plats.
        /// </summary>
        /// <param name="ideePlatService">idée des plats a servir</param>
        public IdeePlatController(IIdeePlatService ideePlatService)
        {
            _ideePlatService = ideePlatService;
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
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
        /// <exception cref="Exception">En cas d'erreur lors de l'ajout</exception>
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
        /// <param name="id">Identifiant de l'idée à modifier</param>
        /// <exception cref="Exception">En cas d'erreur lors de la modification</exception>
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
        /// <param> name="id">Identifiant de l'idée à supprimer</param>
        /// <exception cref="Exception">En cas d'erreur lors de la suppression</exception>
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