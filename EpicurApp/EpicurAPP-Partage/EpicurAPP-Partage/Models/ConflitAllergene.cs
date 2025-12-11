namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un conflit entre les allergies d'un client et les ingrédients d'un menu.
    /// </summary>
    public class ConflitAllergene
    {
        /// <summary>
        /// Identifiant du client concerné.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Nom complet du client.
        /// </summary>
        public string NomClient { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant du menu concerné.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Nom du menu concerné.
        /// </summary>
        public string NomMenu { get; set; } = string.Empty;

        /// <summary>
        /// Liste des allergènes en conflit.
        /// </summary>
        public List<Allergene> AllergenesEnConflit { get; set; } = new List<Allergene>();

        /// <summary>
        /// Liste des ingrédients problématiques présents dans le menu.
        /// </summary>
        public List<Ingredient> IngredientsConcernes { get; set; } = new List<Ingredient>();

        /// <summary>
        /// Niveau de gravité du conflit (Rouge = bloquant, Orange = avertissement).
        /// </summary>
        public NiveauAlerte Niveau { get; set; } = NiveauAlerte.Rouge;

        /// <summary>
        /// Message d'alerte formaté pour l'affichage.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Niveau de gravité de l'alerte allergène.
    /// </summary>
    public enum NiveauAlerte
    {
        /// <summary>
        /// Alerte bloquante - allergène dangereux détecté.
        /// </summary>
        Rouge,

        /// <summary>
        /// Avertissement - allergène potentiel détecté.
        /// </summary>
        Orange
    }
}