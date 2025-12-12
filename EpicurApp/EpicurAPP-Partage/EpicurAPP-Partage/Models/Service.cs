namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Repr�sente un service de restauration.
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
        public string MidiSoir { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant du menu associ� au service.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Statut du service (ex: "Ouvert", "Ferm�", "Complet").
        /// Par d�faut "Ouvert".
        /// </summary>
        public string Statut { get; set; } = "Ouvert";

        /// <summary>
        /// Menu associ� au service (propri�t� de navigation).
        /// Non mapp�e en base de donn�es pour le moment.
        /// </summary>
        public Menu? MenuAssocie { get; set; }

        /// <summary>
        /// Met � jour la date du menu associ� pour qu'elle corresponde � la date de ce service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Lev�e si aucun menu n'est associ�.</exception>
        public void MettreDateMenu()
        {
            if (MenuAssocie == null)
            {
                throw new InvalidOperationException("Impossible d'aligner la date : aucun menu n'est associ� � ce service.");
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