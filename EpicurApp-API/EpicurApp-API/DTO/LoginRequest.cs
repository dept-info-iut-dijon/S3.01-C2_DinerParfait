using System.ComponentModel.DataAnnotations;

namespace EpicurApp_API.DTO
{
    /// <summary>
    /// DTO pour la requête de login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Email de l'utilisateur.
        /// </summary>
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe de l'utilisateur.
        /// </summary>
        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string Password { get; set; } = string.Empty;
    }
}
