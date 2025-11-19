using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        /// <summary>
        /// Service permettant de gérer les opérations sur les clients.
        /// </summary>
        private readonly IClientService _clientService;

        /// <summary>
        /// Constructeur : injection du service client.
        /// </summary>
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Récupère tous les clients enregistrés dans la base.
        /// </summary>
        /// <returns>Liste de clients</returns>
        [HttpGet]
        public IActionResult GetAllClients()
        {
            try
            {
                // Récupération de tous les clients via le service
                List<Client> clients = _clientService.ObtenirTousLesClients();

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
        /// </summary>
        /// <param name="id">Id du client</param>
        /// <returns>Client correspondant</returns>
        [HttpGet("{id}")]
        public IActionResult GetClient(int id)
        {
            try
            {
                // Récupération d'un client par son id
                Client client = _clientService.ObtenirClientParId(id);

                return Ok(client);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erreur lors de la récupération du client.");
            }
        }

        /// <summary>
        /// Crée un nouveau client.
        /// </summary>
        /// <param name="client">Données du client</param>
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
        /// </summary>
        /// <param name="id">Id du client</param>
        /// <param name="client">Données modifiées</param>
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
        /// <returns>Liste des repas triés par date décroissante</returns>
        [HttpGet("{id}/repas")]
        public IActionResult ObtenirHistoriqueRepas(int id)
        {
            try
            {
                // Récupération de l'historique des repas via le service
                var repas = _clientService.ObtenirHistoriqueRepas(id);

                // Si la liste est vide, on renvoie un message approprié
                if (repas == null || repas.Count == 0)
                {
                    return Ok(new
                    {
                        message = "Aucun repas enregistré",
                        repas = new List<object>()
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