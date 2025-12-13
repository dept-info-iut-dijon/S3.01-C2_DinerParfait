using EpicurAPP_Partage.Models;
using System.Collections.Generic;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations sur les menus.
    /// </summary>
    public interface IMenuDAO
    {
        /// <summary>
        /// Ajoute un menu.
        /// </summary>
        /// <param name="menu">Menu à ajouter.</param>
        void AjouterMenu(Menu menu);

        /// <summary>
        /// Récupère un menu par son Id.
        /// </summary>
        /// <param name="id">Id du menu.</param>
        /// <returns>Menu correspondant ou null.</returns>
        Menu? GetById(int id);

        /// <summary>
        /// Récupère tous les menus.
        /// </summary>
        /// <returns>Liste de menus.</returns>
        List<Menu> GetAll();

        /// <summary>
        /// Récupère tous les menus d'un restaurant spécifique.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des menus du restaurant.</returns>
        List<Menu> GetAllByRestaurantId(int restaurantId);

        /// <summary>
        /// Récupère le dernier menu en statut brouillon pour un restaurant donné.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Menu en brouillon ou null.</returns>
        Menu? GetDernierBrouillon(int restaurantId);

        /// <summary>
        /// Récupère tous les menus validés d'un restaurant.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des menus validés du restaurant.</returns>
        List<Menu> GetMenusValides(int restaurantId);

        /// <summary>
        /// Met à jour un menu existant.
        /// </summary>
        /// <param name="menu">Menu à mettre à jour.</param>
        void MettreAJourMenu(Menu menu);

        /// <summary>
        /// Ajoute des plats existants à un menu.
        /// </summary>
        /// <param name="menuId">Id du menu.</param>
        /// <param name="platsIds">Ids des plats à ajouter.</param>
        void AjouterPlatsAuMenu(int menuId, List<int> platIds);

        /// <summary>
        /// Supprime un menu par son Id.
        /// </summary>
        /// <param name="id">Id du menu à supprimer.</param>
        void SupprimerMenu(int id);

        /// <summary>
        /// Vérifie si un menu est associé à un service dans les prochaines 24 heures.
        /// </summary>
        /// <param name="menuId">Id du menu à vérifier.</param>
        /// <returns>True si le menu a un service dans les 24h, False sinon.</returns>
        bool AServiceDansLes24Heures(int menuId);
        
        /// <summary>
        /// Vérifie si un menu est utilisé dans un service.
        /// </summary>
        /// <param name="menuId">Id du menu.</param>
        /// <returns>True si le menu est assigné à au moins un service.</returns>
        bool EstUtilise(int menuId);
    }
}

