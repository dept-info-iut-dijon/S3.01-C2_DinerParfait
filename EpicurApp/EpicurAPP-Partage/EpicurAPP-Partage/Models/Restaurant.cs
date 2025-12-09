using System.ComponentModel.DataAnnotations;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un restaurant dans l'application.
    /// Chaque restaurant a ses propres données (clients, menus, plats, etc.).
    /// </summary>
    public class Restaurant
    {
        /// <summary>
        /// Identifiant unique du restaurant
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du restaurant
        /// </summary>
        [Required(ErrorMessage = "Le nom du restaurant est obligatoire.")]
        public string Nom { get; set; }

        /// <summary>
        /// Ville où se situe le restaurant
        /// </summary>
        [Required(ErrorMessage = "La ville est obligatoire.")]
        public string Ville { get; set; }

        /// <summary>
        /// Liste des utilisateurs associés à ce restaurant
        /// </summary>
        public List<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
    }
}
