using System.ComponentModel.DataAnnotations;

namespace EpicurApp_API.Models
{
    /// <summary>
    /// Représente un client.
    /// Contient les information sur le client
    /// </summary>
    public class Client
    {
        //Nom du client obligatoire
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        //Prenom du client obligatoire
        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public string Prenom { get; set; }

        //Numéro de téléphone du client valide
        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        public string Telephone { get; set; }

        //Email du client obligatoire et valide
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }

        //Allergies du client
        public string Allergies { get; set; }

        //Note supplementaire a propos du client (Préféreces, ...)
        public string Notes { get; set; }
        //Identifiant unique du client
        public int Id { get;  set; }
    }
}

