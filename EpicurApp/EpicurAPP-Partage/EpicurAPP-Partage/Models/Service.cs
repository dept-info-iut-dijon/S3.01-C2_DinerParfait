namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un service de restauration.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Identifiant unique du service.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date du service.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Indique si le service est le midi ou le soir.
        /// Valeurs attendues : "Midi" ou "Soir".
        /// </summary>
        public string MidiSoir { get; set; }

        /// <summary>
        /// Identifiant du menu associé au service.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Statut du service (ex: "Ouvert", "Fermé", "Complet").
        /// Par défaut "Ouvert".
        /// </summary>
        public string Statut { get; set; } = "Ouvert";

        /// <summary>
        /// Menu associé au service (propriété de navigation).
        /// Non mappée en base de données pour le moment.
        /// </summary>
        public Menu? MenuAssocie { get; set; }
    }
}