namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Requête de validation de réservation avec possibilité d'override.
    /// </summary>
    public class ReservationRequest
    {
        /// <summary>
        /// Identifiant du client.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Identifiant du menu associé au service.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Identifiant du service.
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Nombre de couverts.
        /// </summary>
        public int NbCouverts { get; set; } = 1;

        /// <summary>
        /// Indique si le restaurateur force la réservation malgré les conflits.
        /// </summary>
        public bool ForceReservation { get; set; } = false;

        /// <summary>
        /// Note explicative en cas de forçage (ex: "Menu spécial prévu pour lui").
        /// </summary>
        public string? NoteOverride { get; set; }
    }
}