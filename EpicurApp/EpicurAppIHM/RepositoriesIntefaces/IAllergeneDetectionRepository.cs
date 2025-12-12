using EpicurAPP_Partage.Models;
using System.Threading.Tasks;

namespace EpicurAppIHM.RepositoriesIntefaces
{
    /// <summary>
    /// Interface pour le repository de détection des allergènes.
    /// </summary>
    public interface IAllergeneDetectionRepository
    {
        /// <summary>
        /// Détecte les conflits d'allergènes pour un client et un menu.
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="menuId">Identifiant du menu</param>
        /// <returns>Réponse de validation avec les éventuels conflits</returns>
        Task<ValidationReservationResponse?> DetecterConflitAsync(int clientId, int menuId);
    }
}
