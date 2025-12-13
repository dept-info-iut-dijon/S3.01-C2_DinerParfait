using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpicurAppIHM.RepositoriesIntefaces
{
    /// <summary>
    /// Interface pour le repository des menus.
    /// </summary>
    public interface IMenuRepository
    {
        /// <summary>
        /// Récupère tous les menus.
        /// </summary>
        Task<List<Menu>> GetAllAsync();

        /// <summary>
        /// Récupère un menu par son identifiant.
        /// </summary>
        Task<Menu?> GetByIdAsync(int id);

        /// <summary>
        /// Crée un nouveau menu.
        /// </summary>
        Task<Menu> CreateAsync(Menu menu);

        /// <summary>
        /// Met à jour un menu existant.
        /// </summary>
        Task<bool> UpdateAsync(Menu menu);

        /// <summary>
        /// Supprime un menu.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Récupère la liste de courses pour un menu.
        /// </summary>
        Task<List<ElementListeCourse>> GetListeCoursesAsync(int menuId);

        /// <summary>
        /// Récupère le menu brouillon.
        /// </summary>
        Task<Menu?> GetBrouillonAsync();

        /// <summary>
        /// Récupère tous les menus validés (disponibles pour les services).
        /// </summary>
        Task<List<Menu>> GetMenusValidesAsync();

        /// <summary>
        /// Ajoute ou met à jour une note pour un menu.
        /// </summary>
        Task<bool> AddNoteAsync(int menuId, int note);
    }
}
