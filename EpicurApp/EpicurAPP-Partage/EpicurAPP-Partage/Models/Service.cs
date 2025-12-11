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
        public DateTime? Date { get; set; }

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

        /// <summary>
        /// Met à jour la date du menu associé pour qu'elle corresponde à la date de ce service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Levée si aucun menu n'est associé.</exception>
        public void MettreDateMenu()
        {
            if (MenuAssocie == null)
            {
                throw new InvalidOperationException("Impossible d'aligner la date : aucun menu n'est associé à ce service.");
            }

            // On applique la date du service au menu
            // Note : Si Date est nullable (DateTime?), utilise Date.Value
            MenuAssocie.Date = this.Date;
        }

        public bool EstVerrouille
        {
            get
            {
                if (!Date.HasValue) return false;

                
                return (Date.Value - DateTime.Now).TotalHours < 48;
            }
        }

    }
}