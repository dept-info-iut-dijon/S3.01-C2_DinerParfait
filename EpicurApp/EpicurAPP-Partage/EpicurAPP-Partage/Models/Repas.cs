namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un repas pris par un client.
    /// Stocke l'historique des repas avec le menu associé et les retours éventuels.
    /// </summary>
    public class Repas
    {
        /// <summary>
        /// Identifiant unique du repas.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du client qui a pris ce repas.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Identifiant du menu servi lors de ce repas.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Date à laquelle le repas a été pris.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Retours éventuels du client sur ce repas (commentaires, satisfaction, etc.).
        /// </summary>
        public string? Retours { get; set; }

        /// <summary>
        /// Menu associé à ce repas (navigation property).
        /// </summary>
        public Menu? Menu { get; set; }
    }
}
