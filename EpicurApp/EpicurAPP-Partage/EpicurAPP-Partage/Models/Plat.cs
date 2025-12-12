namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un plat avec ses informations et ingrédients.
    /// </summary>
    public class Plat
    {
        /// <summary>
        /// Identifiant unique du plat.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du restaurant auquel appartient ce plat
        /// </summary>
        public int RestaurantId { get; set; }

        /// <summary>
        /// Nom du plat.
        /// </summary>
        public string Nom { get; set; } = string.Empty;

        /// <summary>
        /// Catégorie du plat (Entrée, Plat principal, Dessert, etc.).
        /// </summary>
        public CategoriePlat Categorie { get; set; }

        /// <summary>
        /// Liste des ingrédients principaux du plat.
        /// Essentiel pour la gestion des alertes allergènes.
        /// </summary>
        public List<Ingredient> IngredientsPrincipaux { get; set; } = new List<Ingredient>();

        /// <summary>
        /// Retourne le nom du plat.
        /// </summary>
        public override string ToString()
        {
            return Nom;
        }
    }
}
