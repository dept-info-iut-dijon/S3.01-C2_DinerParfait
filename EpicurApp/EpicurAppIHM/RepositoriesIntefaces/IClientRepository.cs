using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpicurAppIHM.RepositoriesIntefaces
{
    /// <summary>
    /// Interface pour le repository des clients.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Récupère tous les clients.
        /// </summary>
        Task<List<Client>> GetAllAsync();

        /// <summary>
        /// Récupère un client par son identifiant.
        /// </summary>
        Task<Client?> GetByIdAsync(int id);

        /// <summary>
        /// Crée un nouveau client.
        /// </summary>
        Task<Client> CreateAsync(Client client);

        /// <summary>
        /// Met à jour un client existant.
        /// </summary>
        Task<bool> UpdateAsync(Client client);

        /// <summary>
        /// Supprime un client.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Récupère tous les clients avec leurs allergènes.
        /// </summary>
        Task<List<Client>> GetAllWithAllergenesAsync();

        /// <summary>
        /// Récupère un client par son identifiant avec ses allergènes.
        /// </summary>
        Task<Client?> GetByIdWithAllergenesAsync(int id);

        /// <summary>
        /// Recherche des clients par nom.
        /// </summary>
        Task<List<Client>> SearchByNomAsync(string nom);

        /// <summary>
        /// Met à jour les allergènes d'un client.
        /// </summary>
        Task<bool> UpdateAllergenesAsync(int clientId, List<int> allergeneIds);

        /// <summary>
        /// Récupère l'historique des repas d'un client.
        /// </summary>
        Task<List<Repas>> GetRepasAsync(int clientId);
    }
}
