using EpicurApp_API.DAO;
using EpicurApp_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatsController : ControllerBase
    {
        private readonly IPlatDAO platDAO;
        private readonly ILogger<PlatsController> logger;

        public PlatsController(IPlatDAO platDAO, ILogger<PlatsController> logger)
        {
            this.platDAO=platDAO;
            this.logger=logger;
        }

        //GET: api/plats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plat>>> GetAllPlats()
        {
            try
            {
                var plats = await platDAO.GetAllAsync();

                return Ok(plats);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erreur interne du serveur lors de l'accès aux données : {e.Message}");
            }
        }
    }
}
