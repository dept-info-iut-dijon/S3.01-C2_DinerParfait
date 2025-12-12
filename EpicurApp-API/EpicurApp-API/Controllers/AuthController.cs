using System.ComponentModel.DataAnnotations;
using System.Reflection;
using EpicurApp_API.DTO;
using EpicurAppLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpicurApp_API.Controllers
{
    /// <summary>
    /// Controller pour gérer l'authentification des utilisateurs.
    /// </summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRestaurantDAO _restaurantDAO;

        /// <summary>
        /// Constructeur : injection des services d'authentification et restaurant.
        /// </summary>
        public AuthController(IAuthService authService, IRestaurantDAO restaurantDAO)
        {
            _authService = authService;
            _restaurantDAO = restaurantDAO;
        }

        /// <summary>
        /// Endpoint pour se connecter avec email et mot de passe.
        /// </summary>
        /// <param name="request">Requête contenant l'email et le mot de passe.</param>
        /// <returns>Informations de l'utilisateur connecté et de son restaurant.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validation du modèle
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Les champs email et mot de passe sont obligatoires."
                    });
                }

                // Tentative d'authentification
                EpicurAPP_Partage.Models.Utilisateur utilisateur = _authService.Login(request.Email, request.Password);

                if (utilisateur == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Email ou mot de passe invalide."
                    });
                }

                // Récupérer les informations du restaurant
                EpicurAPP_Partage.Models.Restaurant restaurant = _restaurantDAO.GetById(utilisateur.RestaurantId);

                if (restaurant == null)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Erreur lors de la récupération des informations du restaurant."
                    });
                }

                // Retourner les informations de l'utilisateur et du restaurant
                return Ok(new
                {
                    success = true,
                    message = "Connexion réussie.",
                    utilisateur = new
                    {
                        id = utilisateur.Id,
                        email = utilisateur.Email,
                        restaurantId = utilisateur.RestaurantId
                    },
                    restaurant = new
                    {
                        id = restaurant.Id,
                        nom = restaurant.Nom,
                        ville = restaurant.Ville
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de la connexion : " + ex.Message
                });
            }
        }

        /// <summary>
        /// Endpoint pour créer un nouveau compte utilisateur avec son restaurant.
        /// </summary>
        /// <param name="request">Requête contenant les informations d'inscription.</param>
        /// <returns>Informations de l'utilisateur créé et de son restaurant.</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            Console.WriteLine($"[AuthController] Reçu demande inscription pour : {request.Email}");
            try
            {
                // Validation du modèle
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("[AuthController] Modèle invalide");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Veuillez remplir tous les champs obligatoires."
                    });
                }

                // Validation de la confirmation du mot de passe
                if (request.Password != request.ConfirmPassword)
                {
                    Console.WriteLine("[AuthController] Mots de passe ne correspondent pas");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Les mots de passe ne correspondent pas."
                    });
                }

                Console.WriteLine("[AuthController] Appel du service Register...");
                // Création du compte
                var (utilisateur, restaurant) = _authService.Register(
                    request.Email,
                    request.Password,
                    request.RestaurantNom,
                    request.RestaurantVille
                );
                Console.WriteLine($"[AuthController] Succès ! User ID: {utilisateur.Id}, Resto ID: {restaurant.Id}");

                // Retourner les informations (même format que le login)
                return Ok(new
                {
                    success = true,
                    message = "Compte créé avec succès.",
                    utilisateur = new
                    {
                        id = utilisateur.Id,
                        email = utilisateur.Email,
                        restaurantId = utilisateur.RestaurantId
                    },
                    restaurant = new
                    {
                        id = restaurant.Id,
                        nom = restaurant.Nom,
                        ville = restaurant.Ville
                    }
                });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[AuthController] Erreur Argument: {ex.Message}");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[AuthController] Erreur InvalidOperation: {ex.Message}");
                return Conflict(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthController] Erreur CRITIQUE: {ex}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne lors de la création du compte : " + ex.Message
                });
            }
        }
    }
}