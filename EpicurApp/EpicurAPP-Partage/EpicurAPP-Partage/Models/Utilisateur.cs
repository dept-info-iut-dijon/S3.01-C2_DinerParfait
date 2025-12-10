using System.ComponentModel.DataAnnotations;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un utilisateur authentifié de l'application.
    /// Permet à un restaurateur de se connecter et d'accéder aux données de son restaurant.
    /// </summary>
    public class Utilisateur
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email de l'utilisateur - utilisé comme identifiant de connexion
        /// </summary>
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }

        /// <summary>
        /// Hash du mot de passe de l'utilisateur (jamais stocké en clair)
        /// </summary>
        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Identifiant du restaurant associé à cet utilisateur
        /// </summary>
        [Required(ErrorMessage = "L'utilisateur doit être associé à un restaurant.")]
        public int RestaurantId { get; set; }

        /// <summary>
        /// Restaurant associé à cet utilisateur (navigation property)
        /// </summary>
        public Restaurant Restaurant { get; set; }
    }
}
