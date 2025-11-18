namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un ingrédient utilisé dans les plats.
    /// </summary>
    public class Ingredient
    {
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

        public override string ToString()
        {
            return Nom;
        }
    }
}
