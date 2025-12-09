using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des restaurants.
    /// Fournit les opérations CRUD pour les restaurants.
    /// </summary>
    public interface IRestaurantDAO
    {
        /// <summary>
        /// Récupère un restaurant par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du restaurant.</param>
        /// <returns>Le restaurant trouvé ou null si non trouvé.</returns>
        Restaurant? GetById(int id);

        /// <summary>
        /// Récupère tous les restaurants.
        /// </summary>
        /// <returns>Liste de tous les restaurants.</returns>
        List<Restaurant> GetAll();

        /// <summary>
        /// Ajoute un nouveau restaurant dans la base de données.
        /// </summary>
        /// <param name="restaurant">Le restaurant à ajouter.</param>
        void AjouterRestaurant(Restaurant restaurant);

        /// <summary>
        /// Modifie un restaurant existant.
        /// </summary>
        /// <param name="restaurant">Le restaurant avec les données mises à jour.</param>
        void ModifierRestaurant(Restaurant restaurant);

        /// <summary>
        /// Supprime un restaurant de la base de données.
        /// </summary>
        /// <param name="id">Identifiant du restaurant à supprimer.</param>
        void SupprimerRestaurant(int id);
    }
}
