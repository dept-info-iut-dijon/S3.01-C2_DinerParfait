using EpicurApp_API.DAO;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using EpicurAPP_Partage.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public IActionResult GetAllClients()
    {
        try
        {
            var clients = _clientService.ObtenirTousLesClients();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erreur lors de la récupération des clients.");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetClient(int id)
    {
        try
        {
            var client = _clientService.ObtenirClientParId(id);
            return Ok(client);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erreur lors de la récupération du client.");
        }
    }

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

    /// <summary>
    /// Supprime un client de la base de données en fonction de son ID.
    /// </summary>
    /// <param name="id">L'ID du client à supprimer.</param>
    /// <returns>Code 204 si succès, ou un code d'erreur.</returns>
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
