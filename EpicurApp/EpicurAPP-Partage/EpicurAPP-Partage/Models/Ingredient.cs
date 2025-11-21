namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un ingrédient utilisé dans les plats.
    /// </summary>
    public class Ingredient
    {
        /// <summary>
        /// Catégroie de l'ingredient
        /// </summary>
        public enum CategorieIngredient
        {
            FruitLegume,
            ViandePoisson,
            Epicerie,
            Cremerie,
            Boisson,
            Autre
        }
        /// <summary>
        /// Identifiant unique de l'ingrédient.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom de l'ingrédient.
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Description de l'ingrédient.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Affichage de l'ingredient 
        /// </summary>
        /// <returns>Représentation de l'ingredient </returns>
        public override string ToString()
        {
            return Nom;
        }

        /// <summary>
        /// Catégorie de l'ingredient
        /// </summary>
        public CategorieIngredient Categorie { get; set; } = CategorieIngredient.Autre;
    }
}
