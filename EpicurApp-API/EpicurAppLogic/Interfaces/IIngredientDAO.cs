using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des ingrédients
    /// </summary>
    public interface IIngredientDAO
    {
        /// <summary>
        /// Récupère tous les ingrédients
        /// </summary>
        /// <returns>Liste de tous les ingrédients</returns>
        List<Ingredient> GetAll();

        /// <summary>
        /// Récupère un ingrédient par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de l'ingrédient</param>
        /// <returns>L'ingrédient ou null si non trouvé</returns>
        Ingredient? GetById(int id);

        /// <summary>
        /// Récupère les ingrédients d'un plat
        /// </summary>
        /// <param name="platId">Identifiant du plat</param>
        /// <returns>Liste des ingrédients du plat</returns>
        List<Ingredient> GetIngredientsByPlatId(int platId);

        /// <summary>
        /// Ajoute un nouvel ingrédient
        /// </summary>
        /// <param name="ingredient">L'ingrédient à ajouter</param>
        void Add(Ingredient ingredient);

        /// <summary>
        /// Associe des ingrédients à un plat
        /// </summary>
        /// <param name="platId">Identifiant du plat</param>
        /// <param name="ingredientIds">Liste des identifiants des ingrédients</param>
        void AssocierIngredientsAuPlat(int platId, List<int> ingredientIds);
    }
}
