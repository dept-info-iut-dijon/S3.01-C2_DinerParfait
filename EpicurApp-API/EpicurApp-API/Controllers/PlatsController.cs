using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour gérer les plats.
    /// </summary>
    [ApiController]
    [Route("api/plats")]
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
        /// Helper pour récupérer le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <returns>RestaurantId ou null si non fourni.</returns>
        private int? GetRestaurantIdFromHeader()
        {
            if (Request.Headers.TryGetValue("X-Restaurant-Id", out var restaurantIdValue))
            {
                if (int.TryParse(restaurantIdValue, out int restaurantId))
                {
                    return restaurantId;
                }
            }
            return null;
        }

        /// <summary>
        /// Récupère l'ensemble des plats.
        /// Utilise le header X-Restaurant-Id pour filtrer par restaurant.
        /// </summary>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Liste des plats</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Plat>> GetAllPlats()
        {
            try
            {
                // Récupération du RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();

                List<Plat> plats;

                if (restaurantId.HasValue)
                {
                    // Filtrage par restaurant
                    plats = _platDAO.GetAllByRestaurantId(restaurantId.Value);
                }
                else
                {
                    // Sans filtre (pour compatibilité)
                    plats = _platDAO.GetAll();
                }

                // Si aucun plat on renvoie une liste vide pour éviter null
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
        /// Vérifie que le plat appartient au restaurant de l'utilisateur via X-Restaurant-Id.
        /// <param name="id">Identifiant du plat</param>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Le plat correspondant</returns>
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Plat> GetPlatById(int id)
        {
            try
            {
                Plat? plat = _platDAO.GetById(id);

                if (plat == null)
                {
                    return NotFound("Aucun plat trouvé avec l'identifiant " + id);
                }

                // Vérifier que le plat appartient au restaurant de l'utilisateur
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && plat.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden - le plat existe mais n'appartient pas à ce restaurant
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
        /// Utilise le header X-Restaurant-Id pour filtrer par restaurant.
        /// <param name="categorie">Catégorie des plats à récupérer</param>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Liste des plats correspondant à la catégorie</returns>
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

                // Récupération du RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();

                // Filtrer par restaurant d'abord
                List<Plat> tousLesPlats;
                if (restaurantId.HasValue)
                {
                    tousLesPlats = _platDAO.GetAllByRestaurantId(restaurantId.Value);
                }
                else
                {
                    tousLesPlats = _platDAO.GetAll();
                }

                // Puis filtrer manuellement par catégorie
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
        /// Assigne automatiquement le RestaurantId depuis le header X-Restaurant-Id.
        /// <param name="plat">Plat à créer</param>
        /// <exception cref="Exception">En cas d'erreur lors de la création</exception>
        /// <returns>Le plat créé</returns>
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
                // Assigner automatiquement le RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue)
                {
                    plat.RestaurantId = restaurantId.Value;
                }

                // Ajout en base
                _platDAO.Add(plat);

                // Retourne le plat créé
                return Ok(plat);
            }
            catch (Exception exception)
            {
                return StatusCode(500, "Erreur lors de la création : " + exception.Message);
            }
        }
    }
}
