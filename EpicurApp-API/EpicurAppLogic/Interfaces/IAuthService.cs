using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations du service d'authentification.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authentifie un utilisateur avec son email et son mot de passe.
        /// </summary>
        /// <param name="email">L'email de l'utilisateur.</param>
        /// <param name="password">Le mot de passe en clair.</param>
        /// <returns>L'utilisateur authentifié ou null si les identifiants sont invalides.</returns>
        Utilisateur? Login(string email, string password);

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
        (Utilisateur utilisateur, Restaurant restaurant) Register(string email, string password, string restaurantNom, string restaurantVille);

        /// <summary>
        /// Valide qu'un mot de passe respecte les critères de sécurité.
        /// </summary>
        /// <param name="password">Le mot de passe à valider.</param>
        /// <returns>True si valide, false sinon.</returns>
        bool ValidatePasswordStrength(string password);

        /// <summary>
        /// Hash un mot de passe.
        /// </summary>
        /// <param name="password">Le mot de passe en clair à hasher.</param>
        /// <returns>Le hash du mot de passe.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Vérifie qu'un mot de passe correspond au hash.
        /// </summary>
        /// <param name="password">Le mot de passe en clair.</param>
        /// <param name="hash">Le hash à comparer.</param>
        /// <returns>True si le mot de passe est valide, false sinon.</returns>
        bool VerifyPassword(string password, string hash);
    }
}
