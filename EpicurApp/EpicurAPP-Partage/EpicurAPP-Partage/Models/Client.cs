using System.ComponentModel.DataAnnotations;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un client.
    /// Contient les information sur le client
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Nom du client obligatoire
        /// </summary>
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Prenom du client obligatoire
        /// </summary>
        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string Prenom { get; set; } = string.Empty;

        /// <summary>
        /// Numéro de téléphone du client valide
        /// </summary>
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string Telephone { get; set; } = string.Empty;

        /// <summary>
        /// Email du client obligatoire et valide
        /// </summary>
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Allergenes du client
        /// </summary>
        public List<Allergene> Allergenes { get; set; } = new List<Allergene>();

        /// <summary>
        /// Identifiant unique du client
        /// </summary>
        public int Id { get;  set; }

        /// <summary>
        /// Identifiant du restaurant auquel appartient ce client
        /// </summary>
        public int RestaurantId { get; set; }

        /// <summary>
        /// Liste des plats que le client n'apprécie pas.
        /// </summary>
        public List<Plat> PlatsNonApprecies { get; set; } = new List<Plat>();

        /// <summary>
        /// Préférences alimentaires du client.
        /// </summary>
        public string Preferences { get; set; } = string.Empty;
    }
}

