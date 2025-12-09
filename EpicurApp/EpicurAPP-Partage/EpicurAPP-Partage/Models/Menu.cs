namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un menu composé de plusieurs plats.
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Identifiant unique du menu.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du restaurant auquel appartient ce menu
        /// </summary>
        public int RestaurantId { get; set; }

        /// <summary>
        /// Nom du menu.
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Date du menu.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Statut du menu (Brouillon, Validé, etc.).
        /// </summary>
        public string Statut { get; set; } = "Brouillon";

        /// <summary>
        /// Liste des éléments (plats) du menu.
        /// Permet d'avoir plusieurs plats de la même catégorie.
        /// </summary>
        public List<ElementMenu> Elements { get; set; } = new List<ElementMenu>();
    }
}
