namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Réponse de validation d'une réservation.
    /// </summary>
    public class ValidationReservationResponse
    {
        /// <summary>
        /// Indique si la réservation est validée.
        /// </summary>
        public bool EstValide { get; set; }

        /// <summary>
        /// Indique si des conflits d'allergènes ont été détectés.
        /// </summary>
        public bool ADesConflits { get; set; }

        /// <summary>
        /// Liste des conflits détectés.
        /// </summary>
        public List<ConflitAllergene> Conflits { get; set; } = new List<ConflitAllergene>();

        /// <summary>
        /// Indique si la réservation a été forcée malgré les conflits.
        /// </summary>
        public bool EstForcee { get; set; }

        /// <summary>
        /// Note d'override si la réservation a été forcée.
        /// </summary>
        public string? NoteOverride { get; set; }

        /// <summary>
        /// Message global de validation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant de la réservation créée (si succès).
        /// </summary>
        public int? ReservationId { get; set; }
    }
}