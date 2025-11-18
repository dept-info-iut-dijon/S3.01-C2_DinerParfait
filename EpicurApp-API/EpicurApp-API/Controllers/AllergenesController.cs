using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.DAO.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AllergenesController : ControllerBase
    {
        /// <summary>
        /// DAO permettant d'accéder aux données des allergènes.
        /// </summary>
        private readonly AllergeneDAO _allergeneDAO;

        /// <summary>
        /// Constructeur : injection du DAO des allergènes.
        /// </summary>
        public AllergenesController(AllergeneDAO allergeneDAO)
        {
            _allergeneDAO = allergeneDAO;
        }

        /// <summary>
        /// Récupère tous les allergènes disponibles.
        /// </summary>
        /// <returns>Liste des allergènes</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Appel au DAO pour récupérer la liste complète
                List<Allergene> allergenes = _allergeneDAO.GetAll();

                // Renvoie un code 200 (OK) avec la liste
                return Ok(allergenes);
            }
            catch (Exception exception)
            {
                // En cas d’erreur → code 500
                return StatusCode(500, "Erreur lors de la récupération des allergènes : " + exception.Message);
            }
        }

        /// <summary>
        /// Ajoute un nouvel allergène.
        /// </summary>
        /// <param name="allergene">Allergène à ajouter</param>
        /// <returns>Allergène créé</returns>
        [HttpPost]
        public IActionResult AjouterAllergene([FromBody] Allergene allergene)
        {
            try
            {
                // Appel au DAO pour ajouter l’allergène en base
                _allergeneDAO.AjouterAllergene(allergene);

                // 201 = Created
                return StatusCode(201, allergene);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur lors de l’ajout de l’allergène : " + exception.Message);
            }
        }
    }
}
