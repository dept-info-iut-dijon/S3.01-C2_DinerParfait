using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpicurAppIHM.RepositoriesIntefaces
{
    /// <summary>
    /// Interface pour le repository des plats.
    /// </summary>
    public interface IPlatRepository
    {
        /// <summary>
        /// Récupère tous les plats.
        /// </summary>
        Task<List<Plat>> GetAllAsync();

        /// <summary>
        /// Récupère un plat par son identifiant.
        /// </summary>
        Task<Plat?> GetByIdAsync(int id);

        /// <summary>
        /// Crée un nouveau plat.
        /// </summary>
        Task<Plat> CreateAsync(Plat plat);

        /// <summary>
        /// Met à jour un plat existant.
        /// </summary>
        Task<bool> UpdateAsync(Plat plat);

        /// <summary>
        /// Supprime un plat.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Récupère tous les plats avec leurs ingrédients.
        /// </summary>
        Task<List<Plat>> GetAllWithIngredientsAsync();

        /// <summary>
        /// Récupère un plat par son identifiant avec ses ingrédients.
        /// </summary>
        Task<Plat?> GetByIdWithIngredientsAsync(int id);
    }
}
