using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using System.Security.Cryptography;
using System.Text;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Service pour gérer l'authentification des utilisateurs.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUtilisateurDAO _utilisateurDAO;
        private readonly IRestaurantDAO _restaurantDAO;

        /// <summary>
        /// Initialise une nouvelle instance de AuthService.
        /// </summary>
        /// <param name="utilisateurDAO">DAO pour accéder aux utilisateurs.</param>
        /// <param name="restaurantDAO">DAO pour accéder aux restaurants.</param>
        public AuthService(IUtilisateurDAO utilisateurDAO, IRestaurantDAO restaurantDAO)
        {
            _utilisateurDAO = utilisateurDAO;
            _restaurantDAO = restaurantDAO;
        }

        /// <summary>
        /// Authentifie un utilisateur avec son email et son mot de passe.
        /// </summary>
        /// <param name="email">L'email de l'utilisateur.</param>
        /// <param name="password">Le mot de passe en clair.</param>
        /// <returns>L'utilisateur authentifié ou null si les identifiants sont invalides.</returns>
        public Utilisateur? Login(string email, string password)
        {
            // Récupérer l'utilisateur par email
            Utilisateur? utilisateur = _utilisateurDAO.GetByEmail(email);

            if (utilisateur == null)
            {
                return null;
            }

            // Vérifier le mot de passe
            if (!VerifyPassword(password, utilisateur.PasswordHash))
            {
                return null;
            }

            return utilisateur;
        }

        /// <summary>
        /// Hash un mot de passe en utilisant SHA256.
        /// </summary>
        /// <param name="password">Le mot de passe en clair à hasher.</param>
        /// <returns>Le hash du mot de passe en Base64.</returns>
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Vérifie qu'un mot de passe correspond au hash.
        /// </summary>
        /// <param name="password">Le mot de passe en clair.</param>
        /// <param name="hash">Le hash à comparer.</param>
        /// <returns>True si le mot de passe est valide, false sinon.</returns>
        public bool VerifyPassword(string password, string hash)
        {
            string passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        /// <summary>
        /// Valide qu'un mot de passe respecte les critères de sécurité.
        /// </summary>
        /// <param name="password">Le mot de passe à valider.</param>
        /// <returns>True si valide, false sinon.</returns>
        public bool ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        /// <summary>
        /// Enregistre un nouvel utilisateur et crée son restaurant.
        /// </summary>
        /// <param name="email">Email de l'utilisateur.</param>
        /// <param name="password">Mot de passe en clair.</param>
        /// <param name="restaurantNom">Nom du restaurant.</param>
        /// <param name="restaurantVille">Ville du restaurant.</param>
        /// <returns>L'utilisateur créé avec son restaurant associé.</returns>
        /// <exception cref="ArgumentException">Si le mot de passe ne respecte pas les critères de sécurité.</exception>
        /// <exception cref="InvalidOperationException">Si l'email existe déjà.</exception>
        public (Utilisateur utilisateur, Restaurant restaurant) Register(
            string email,
            string password,
            string restaurantNom,
            string restaurantVille)
        {
            // 1. Valider la force du mot de passe
            if (!ValidatePasswordStrength(password))
            {
                throw new ArgumentException(
                    "Le mot de passe doit contenir au moins 8 caractères, " +
                    "une majuscule, une minuscule, un chiffre et un caractère spécial."
                );
            }

            // 2. Vérifier que l'email n'existe pas déjà
            var existingUser = _utilisateurDAO.GetByEmail(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Un compte avec cet email existe déjà.");
            }

            // 3. Créer le restaurant
            var restaurant = new Restaurant
            {
                Nom = restaurantNom,
                Ville = restaurantVille
            };
            _restaurantDAO.AjouterRestaurant(restaurant);

            // 4. Créer l'utilisateur avec le mot de passe hashé
            var utilisateur = new Utilisateur
            {
                Email = email,
                PasswordHash = HashPassword(password),
                RestaurantId = restaurant.Id
            };
            _utilisateurDAO.AjouterUtilisateur(utilisateur);

            return (utilisateur, restaurant);
        }
    }
}
