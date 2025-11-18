namespace EpicurApp_API.Models
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
        /// Nom du menu.
        /// </summary>
        public string Nom {  get; set; }

        /// <summary>
        /// Date du menu.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Statut du menu (Brouillon, Validé, etc.).
        /// </summary>
        public string Statut { get; set; } = "Brouillon";

        /// <summary>
        /// Plat amuse-bouche du menu.
        /// </summary>
        public Plat? AmuseBouche { get; set; }

        /// <summary>
        /// Boisson apéritive du menu.
        /// </summary>
        public Plat? BoissonAperitif { get; set; }

        /// <summary>
        /// Entrée du menu.
        /// </summary>
        public Plat? Entree { get; set; }

        /// <summary>
        /// Plat principal du menu.
        /// </summary>
        public Plat? PlatPrincipal { get; set; }

        /// <summary>
        /// Vin du menu.
        /// </summary>
        public Plat? Vin { get; set; }

        /// <summary>
        /// Fromage du menu.
        /// </summary>
        public Plat? Fromage { get; set; }

        /// <summary>
        /// Dessert du menu.
        /// </summary>
        public Plat? Dessert { get; set; }
    }
}
