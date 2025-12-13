using System.ComponentModel.DataAnnotations;

namespace EpicurApp_API.DTO
{
    /// <summary>
    /// DTO pour la requête d'inscription.
    /// </summary>
    public class RegisterRequest
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
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation du mot de passe.
        /// </summary>
        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Nom du restaurant.
        /// </summary>
        [Required(ErrorMessage = "Le nom du restaurant est obligatoire.")]
        public string RestaurantNom { get; set; } = string.Empty;

        /// <summary>
        /// Ville du restaurant.
        /// </summary>
        [Required(ErrorMessage = "La ville du restaurant est obligatoire.")]
        public string RestaurantVille { get; set; } = string.Empty;
    }
}
