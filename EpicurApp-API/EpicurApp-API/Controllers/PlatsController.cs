using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlatsController : Controller
    {
        /// <summary>
        /// DAO permettant d'accéder aux données des plats.
        /// </summary>
        private readonly IPlatDAO _platDAO;

        /// <summary>
        /// Constructeur : injection du DAO des plats.
        /// </summary>
        public PlatsController(IPlatDAO platDAO)
        {
            _platDAO = platDAO;
        }

      

        /// <summary>
        /// Récupère l'ensemble des plats.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Plat>> GetAllPlats()
        {
            try
            {
                // Récupération de tous les plats
                List<Plat> plats = _platDAO.GetAll();

                // Si aucun plat → renvoyer une liste vide pour éviter null
                if (plats == null || plats.Count == 0)
                {
                    return Ok(new List<Plat>());
                }

                return Ok(plats);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur interne du serveur : " + exception.Message);
            }
        }

     

        /// <summary>
        /// Récupère un plat via son identifiant.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Plat> GetPlatById(int id)
        {
            try
            {
                Plat plat = _platDAO.GetById(id);

                if (plat == null)
                {
                    return NotFound("Aucun plat trouvé avec l'identifiant " + id);
                }

                return Ok(plat);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur interne du serveur : " + exception.Message);
            }
        }

     

        /// <summary>
        /// Récupère la liste des plats appartenant à une catégorie donnée.
        /// </summary>
        [HttpGet("categorie/{categorie}")]
        public ActionResult<IEnumerable<Plat>> GetPlatsByCategorie(string categorie)
        {
            try
            {
                // Vérifier que la chaîne correspond bien à une valeur de l'énumération
                CategoriePlat categorieEnum;
                bool conversion = Enum.TryParse<CategoriePlat>(categorie, true, out categorieEnum);

                if (!conversion)
                {
                    return BadRequest("Catégorie invalide : " + categorie);
                }

                // Filtrer manuellement pour éviter LINQ complexe
                List<Plat> tousLesPlats = _platDAO.GetAll();
                List<Plat> platsFiltres = new List<Plat>();

                for (int i = 0; i < tousLesPlats.Count; i++)
                {
                    if (tousLesPlats[i].Categorie == categorieEnum)
                    {
                        platsFiltres.Add(tousLesPlats[i]);
                    }
                }

                return Ok(platsFiltres);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur interne du serveur : " + exception.Message);
            }
        }

    

        /// <summary>
        /// Crée un nouveau plat.
        /// </summary>
        [HttpPost]
        public ActionResult<Plat> CreatePlat([FromBody] Plat plat)
        {
            // Vérification du modèle
            if (plat == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Ajout en base
                _platDAO.Add(plat);

                // Retourne le plat créé avec un code 201
                return CreatedAtAction(nameof(GetPlatById), new { id = plat.Id }, plat);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur lors de la création : " + exception.Message);
            }
        }
    }
}
