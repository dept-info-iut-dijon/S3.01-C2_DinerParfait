using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour gérer les allergènes.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AllergenesController : ControllerBase
    {
        /// <summary>
        /// Service permettant de gérer les allergènes.
        /// </summary>
        private readonly IAllergeneService _allergeneService;

        /// <summary>
        /// Constructeur : injection du service des allergènes.
        /// </summary>
        public AllergenesController(IAllergeneService allergeneService)
        {
            _allergeneService = allergeneService;
        }

        /// <summary>
        /// Récupère tous les allergènes disponibles.
        /// </summary>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Liste des allergènes</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Appel au service pour récupérer la liste complète
                List<Allergene> allergenes = _allergeneService.GetAll();

                // Renvoie un code 200 (OK) avec la liste
                return Ok(allergenes);
            }
            catch (Exception exception)
            {
                // En cas d'erreur → code 500
                return StatusCode(500, "Erreur lors de la récupération des allergènes : " + exception.Message);
            }
        }

        /// <summary>
        /// Ajoute un nouvel allergène.
        /// </summary>
        /// <param name="allergene">Allergène à ajouter</param>
        /// <exception cref="Exception">Si l'allergène est null</exception>
        /// <returns>Allergène créé</returns>
        [HttpPost]
        public IActionResult AjouterAllergene([FromBody] Allergene allergene)
        {
            try
            {
                // Appel au service pour ajouter l'allergène en base
                _allergeneService.AjouterAllergene(allergene);

                // 201 = Created
                return StatusCode(201, allergene);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur lors de l'ajout de l'allergène : " + exception.Message);
            }
        }
    }
}
