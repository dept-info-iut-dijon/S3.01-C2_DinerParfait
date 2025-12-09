using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour gérer les clients.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        /// <summary>
        /// Service permettant de gérer les opérations sur les clients.
        /// </summary>
        private readonly IClientService _clientService;
        private readonly IClientDAO _clientDAO;

        /// <summary>
        /// Constructeur : injection du service client et du DAO.
        /// </summary>
        public ClientController(IClientService clientService, IClientDAO clientDAO)
        {
            _clientService = clientService;
            _clientDAO = clientDAO;
        }

        /// <summary>
        /// Helper pour récupérer le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <returns>RestaurantId ou null si non fourni.</returns>
        private int? GetRestaurantIdFromHeader()
        {
            Microsoft.Extensions.Primitives.StringValues restaurantIdValue;

            if (Request.Headers.TryGetValue("X-Restaurant-Id", out restaurantIdValue))
            {
                int restaurantId;
                if (int.TryParse(restaurantIdValue, out restaurantId))
                {
                    return restaurantId;
                }
            }

            return null;
        }

        /// <summary>
        /// Récupère tous les clients enregistrés dans la base.
        /// Utilise le header X-Restaurant-Id pour filtrer par restaurant.
        /// </summary
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Liste de clients</returns>
        [HttpGet]
        public IActionResult GetAllClients()
        {
            try
            {
                // Récupération du RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();

                List<Client> clients;

                if (restaurantId.HasValue)
                {
                    // Filtrage par restaurant
                    clients = _clientDAO.GetAllByRestaurantId(restaurantId.Value);
                }
                else
                {
                    // Sans filtre (pour compatibilité)
                    clients = _clientService.ObtenirTousLesClients();
                }

                // Renvoie un code 200 avec les données
                return Ok(clients);
            }
            catch (Exception)
            {
                // En cas d'erreur, renvoie un code 500
                return StatusCode(500, "Erreur lors de la récupération des clients.");
            }
        }

        /// <summary>
        /// Récupère un client grâce à son identifiant.
        /// Vérifie que le client appartient au restaurant de l'utilisateur via X-Restaurant-Id.
        /// </summary>
        /// <param name="id">Id du client</param>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Client correspondant</returns>
        [HttpGet("{id}")]
        public IActionResult GetClient(int id)
        {
            try
            {
                // Récupération d'un client par son id
                Client client = _clientService.ObtenirClientParId(id);

                if (client == null)
                {
                    return NotFound($"Client avec l'ID {id} introuvable");
                }

                // Vérifier que le client appartient au restaurant de l'utilisateur
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && client.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden
                }

                return Ok(client);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erreur lors de la récupération du client.");
            }
        }

        /// <summary>
        /// Crée un nouveau client.
        /// Assigne automatiquement le RestaurantId depuis le header X-Restaurant-Id.
        /// </summary>
        /// <param name="client">Données du client</param
        /// <exception cref="InvalidFieldException">Si un champ obligatoire est invalide</exception>
        /// <exception cref="ApplicationException">En cas d'erreur applicative (DAO, service,…)</exception>
        /// <exception cref="Exception">Toute autre erreur interne</exception>
        /// <returns>Client créé</returns>
        [HttpPost]
        public IActionResult CreerClient([FromBody] Client client)
        {
            try
            {
                // Vérifie que le modèle reçu respecte les règles de validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Assigner automatiquement le RestaurantId depuis le header
                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue)
                {
                    client.RestaurantId = restaurantId.Value;
                }

                // Appel du service pour l'ajouter
                _clientService.AjouterClient(client);

                // Renvoie le client créé
                return Ok(client);
            }
            catch (InvalidFieldException ex)
            {
                // Si un champ obligatoire est invalide → erreur 400
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                // Erreur applicative (DAO, service,…)
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                // Toute autre erreur interne
                return StatusCode(500, "Erreur interne : " + ex.Message);
            }
        }

        /// <summary>
        /// Modifie un client existant (US 1.3).
        /// Vérifie que le client appartient au restaurant de l'utilisateur.
        /// </summary>
        /// <param name="id">Id du client</param>
        /// <param name="client">Données modifiées</param
        /// <exception cref="InvalidFieldException">Si un champ obligatoire est invalide</exception>
        /// <exception cref="Exception">Toute autre erreur interne</exception>
        /// <returns>Client modifié</returns>
        [HttpPut("{id}")]
        public IActionResult ModifierClient(int id, [FromBody] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Vérifier que le client appartient au restaurant de l'utilisateur
                Client existingClient = _clientService.ObtenirClientParId(id);
                if (existingClient == null)
                {
                    return NotFound($"Client avec l'ID {id} introuvable");
                }

                int? restaurantId = GetRestaurantIdFromHeader();
                if (restaurantId.HasValue && existingClient.RestaurantId != restaurantId.Value)
                {
                    return Forbid(); // 403 Forbidden
                }

                // S'assurer que le RestaurantId ne change pas
                client.RestaurantId = existingClient.RestaurantId;

                _clientService.ModifierClient(client);
                return Ok(client);
            }
            catch (InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la modification : " + ex.Message);
            }
        }

        /// <summary>
        /// Associe une liste d'allergènes à un client.
        /// </summary>
        /// <param name="id">Id du client</param>
        /// <param name="allergeneIds">Liste des Ids d'allergènes</param>
        /// <exception cref="Exception">Toute autre erreur interne</exception>
        /// <returns>Résultat HTTP</returns>
        [HttpPost("{id}/allergenes")]
        public IActionResult AssocierAllergenes(int id, [FromBody] List<int> allergeneIds)
        {
            try
            {
                // Appel du service pour associer les allergènes
                _clientService.AjouterAllergenesAuClient(id, allergeneIds);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Erreur lors de l'association des allergènes au client : " + ex.Message
                );
            }
        }

        /// <summary>
        /// Récupère l'historique des repas d'un client (US 1.5).
        /// </summary>
        /// <param name="id">Id du client</param>
        /// <exception cref="Exception">En cas d'erreur lors de la récupération</exception>
        /// <returns>Liste des repas triés par date décroissante</returns>
        [HttpGet("{id}/repas")]
        public IActionResult ObtenirHistoriqueRepas(int id)
        {
            try
            {
                // Récupération de l'historique des repas via le service
                List<Repas> repas = _clientService.ObtenirHistoriqueRepas(id);

                // Si la liste est vide, on renvoie un message approprié
                if (repas == null || repas.Count == 0)
                {
                    List<object> listeVide = new List<object>();

                    return Ok(new
                    {
                        message = "Aucun repas enregistré",
                        repas = listeVide
                    });
                }

                return Ok(repas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération de l'historique des repas : {ex.Message}");
            }
        }

        /// <summary>
        /// Supprime un client de la base de données en fonction de son ID (US 1.4).
        /// </summary>
        /// <param name="id">L'ID du client à supprimer.</param>
        /// <exception cref="Exception">En cas d'erreur lors de la suppression</exception>
        /// <returns>Code 200 si succès, ou un code d'erreur.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            try
            {
                _clientService.Delete(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
