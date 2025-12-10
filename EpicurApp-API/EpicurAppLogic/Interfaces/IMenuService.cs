using EpicurAPP_Partage.Models;
using System.Collections.Generic;

namespace EpicurAppLogic.Interfaces
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

        /// <summary>
        /// Génère la liste de courses pour un menu donné.
        /// </summary>
        /// <param name="menuId">Menu dont on veut la liste de course</param>
        /// <returns>Liste de course du menu</returns>
        List<ElementListeCourse> GenererListeCourses(int menuId);

        /// <summary>
        /// Supprime un menu par son Id.
        /// </summary>
        /// <param name="id">Id du menu à supprimer.</param>
        void SupprimerMenu(int id);

        /// <summary>
        /// Met à jour la note d'un menu.
        /// </summary>
        /// <param name="menuId">Id du menu a update</param>
        /// <param name="note">Note attribué</param>
        void MettreAJourNoteDuMenu(int menuId, int note);
        /// <summary>
        /// Ajoute une note à un menu.
        /// </summary>
        /// <param name="menuId">Id du menu avec la note</param>
        /// <param name="note">Note attribué</param>
        void AjouterNoteAuMenu(int menuId, int note);
    }
}

