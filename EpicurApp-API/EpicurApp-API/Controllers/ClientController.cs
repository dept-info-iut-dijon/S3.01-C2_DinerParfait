using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des clients
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Récupère tous les clients
        /// </summary>
        /// <returns>Liste des clients</returns>
        [HttpGet]
        public IActionResult GetAllClients()
        {
            try
            {
                List<Client> clients = _clientService.ObtenirTousLesClients();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de la récupération des clients.");
            }
        }

        /// <summary>
        /// Récupère un client par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns>Le client correspondant</returns>
        [HttpGet("{id}")]
        public IActionResult GetClient(int id)
        {
            try
            {
                Client client = _clientService.ObtenirClientParId(id);
                return Ok(client);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erreur lors de la récupération du client.");
            }
        }

        /// <summary>
        /// Crée un nouveau client
        /// </summary>
        /// <param name="client">Données du client à créer</param>
        /// <returns>Le client créé</returns>
        [HttpPost]
        public IActionResult CreerClient([FromBody] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _clientService.AjouterClient(client);
                return Ok(client);
            }
            catch (InvalidFieldException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur interne : " + ex.Message);
            }
        }

        /// <summary>
        /// Associe des allergènes à un client
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpPost("{id}/allergenes")]
        public IActionResult AssocierAllergenes(int id, [FromBody] List<int> allergeneIds)
        {
            try
            {
                _clientService.AjouterAllergenesAuClient(id, allergeneIds);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erreur lors de l'association des allergènes au client : " + ex.Message);
            }
        }
    }
}
