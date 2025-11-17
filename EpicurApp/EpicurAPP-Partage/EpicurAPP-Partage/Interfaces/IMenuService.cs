using EpicurApp_API.Models;
using System.Collections.Generic;

namespace EpicurAPP_Partage.Interfaces
{
    /// <summary>
    /// Interface définissant les règles métiers pour la gestion des menus.
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Crée un menu en appliquant les règles métiers.
        /// </summary>
        /// <param name="menu">Menu à créer.</param>
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
        /// Récupère le dernier menu en statut brouillon.
        /// </summary>
        /// <returns>Menu en brouillon ou null.</returns>
        Menu? GetDernierBrouillon();

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
    }
}

