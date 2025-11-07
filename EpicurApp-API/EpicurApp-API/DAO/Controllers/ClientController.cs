using EpicurApp_API.Models;
using EpicurAPP_Partage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EpicurApp_API.DAO.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : Controller
    {

        private IClientDAO clientDAO;

        public ClientController(IClientDAO clientDAO)
        {
            this.clientDAO = clientDAO;
        }

        /// <summary>
        /// Crée un nouveau client
        /// </summary>
        /// <param name="client">Les données du client à créer</param>
        /// <returns>Une réponse HTTP</returns>
        [HttpPost]
        public IActionResult CreateClient(Client client)
        {
            // On verifie que les données rentrées soient valides.
            if (!ModelState.IsValid)
            {
                // Si invalide erreur 400 Bad Request
                return BadRequest(ModelState);
            }

            try
            {
                clientDAO.AjouterClient(client);
                return StatusCode(StatusCodes.Status201Created, client);
            }
            catch (Exception ex)
            {
                // Sinon renvoie une erreur 500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, "Une erreur est survenue lors de la création du client.");
            }
        }
        /// <summary>
        /// Récupère la liste de tous les clients
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            try
            {
                var clients = await clientDAO.GetAllAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClientById(int id)
        {
            try
            {
                var client = await clientDAO.GetByIdWithHistoryAsync(id);

                if (client == null)
                {
                    return NotFound(); // Renvoie 404
                }

                // Renvoie 200 avec le client et sa liste HistoriqueRepas
                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}

