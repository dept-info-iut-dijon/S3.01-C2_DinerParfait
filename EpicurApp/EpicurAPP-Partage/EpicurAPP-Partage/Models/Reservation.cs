namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente une réservation effectuée par un client pour un service.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du service réservé.
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Identifiant du client qui effectue la réservation.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Nombre de couverts réservés.
        /// </summary>
        public int NbCouverts { get; set; }

        /// <summary>
        /// Nom du client (pour affichage uniquement).
        /// Non mappé en base de données.
        /// </summary>
        public string? NomClient { get; set; }

        /// <summary>
        /// Prénom du client (pour affichage uniquement).
        /// Non mappé en base de données.
        /// </summary>
        public string? PrenomClient { get; set; }
    }
}