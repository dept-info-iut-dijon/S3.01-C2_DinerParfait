using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpicurAppIHM.RepositoriesIntefaces
{
    /// <summary>
    /// Interface pour le repository des allergènes.
    /// </summary>
    public interface IAllergeneRepository
    {
        /// <summary>
        /// Récupère tous les allergènes.
        /// </summary>
        Task<List<Allergene>> GetAllAsync();

        /// <summary>
        /// Récupère un allergène par son identifiant.
        /// </summary>
        Task<Allergene?> GetByIdAsync(int id);

        /// <summary>
        /// Crée un nouvel allergène.
        /// </summary>
        Task<Allergene> CreateAsync(Allergene allergene);

        /// <summary>
        /// Met à jour un allergène existant.
        /// </summary>
        Task<bool> UpdateAsync(Allergene allergene);

        /// <summary>
        /// Supprime un allergène.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
