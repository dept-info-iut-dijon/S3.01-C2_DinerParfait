using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpicurAppIHM.RepositoriesIntefaces
{
    /// <summary>
    /// Interface pour le repository des idées de plats.
    /// </summary>
    public interface IIdeePlatRepository
    {
        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        Task<List<IdeePlat>> GetAllAsync();

        /// <summary>
        /// Récupère une idée de plat par son identifiant.
        /// </summary>
        Task<IdeePlat?> GetByIdAsync(int id);

        /// <summary>
        /// Crée une nouvelle idée de plat.
        /// </summary>
        Task<IdeePlat> CreateAsync(IdeePlat ideePlat);

        /// <summary>
        /// Met à jour une idée de plat existante.
        /// </summary>
        Task<bool> UpdateAsync(IdeePlat ideePlat);

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
