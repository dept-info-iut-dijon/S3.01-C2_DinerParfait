namespace EpicurApp_API.Models
{
    /// <summary>
    /// Représente un client.
    /// Contient les information sur le client
    /// </summary>
    public class Client
    {
        //Nom du client
        public string Nom { get; set; }

        //Prenom du client
        public string Prenom { get; set; }

        //Numéro de téléphone du client
        public string Telephone { get; set; }

        //Email du client
        public string Email { get; set; }

        //Allergies du client
        public string Allergies { get; set; }

        //Note supplementaire a propos du client (Préféreces, ...)
        public string Note { get; set; }
    }
}

