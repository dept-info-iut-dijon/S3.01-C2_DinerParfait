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

        /// <summary>
        /// Initialise une nouvelle instance de AuthService.
        /// </summary>
        /// <param name="utilisateurDAO">DAO pour accéder aux utilisateurs.</param>
        public AuthService(IUtilisateurDAO utilisateurDAO)
        {
            _utilisateurDAO = utilisateurDAO;
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
    }
}
