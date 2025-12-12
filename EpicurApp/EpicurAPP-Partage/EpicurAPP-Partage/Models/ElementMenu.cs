namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un élément (plat) d'un menu avec sa position.
    /// Permet d'avoir plusieurs plats de la même catégorie dans un menu.
    /// </summary>
    public class ElementMenu
    {
        /// <summary>
        /// Identifiant unique de l'élément.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du menu auquel appartient cet élément.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Identifiant du plat.
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// Plat associé à cet élément.
        /// </summary>
        public Plat Plat { get; set; } = null!;

        /// <summary>
        /// Catégorie du plat dans le menu (Entrée, Plat Principal, Dessert, etc.).
        /// </summary>
        public CategoriePlat Categorie { get; set; }

        /// <summary>
        /// Ordre d'apparition du plat dans sa catégorie.
        /// Permet d'avoir plusieurs desserts, plusieurs vins, etc.
        /// </summary>
        public int Ordre { get; set; }
    }
}
