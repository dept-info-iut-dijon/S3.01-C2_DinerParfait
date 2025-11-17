using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les règles métiers pour la gestion des clients
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Ajoute un client en appliquant les règles métiers
        /// </summary>
        /// <param name="client">Client à ajouter</param>
        void AjouterClient(Client client);

        /// <summary>
        /// Récupère tous les clients
        /// </summary>
        /// <returns>Liste de tous les clients</returns>
        List<Client> ObtenirTousLesClients();

        /// <summary>
        /// Récupère un client par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns>Le client correspondant</returns>
        Client ObtenirClientParId(int id);

        /// <summary>
        /// Associe des allergènes à un client
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);
    }
}
