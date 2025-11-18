using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations du service liées à la gestion des clients.
    /// </summary>
    /// <remarks>
    public interface IClientService
    {
        /// <summary>
        /// Ajoute un nouveau client dans la base de données.
        /// </summary>
        /// <param name="client">L'objet <see cref="Client"/> contenant les informations du client à ajouter.</param>
        void AjouterClient(Client client);

        List<Client> ObtenirTousLesClients();
        Client ObtenirClientParId(int id);
        Task<Client> ObtenirClientAvecHistoriqueAsync(int id);
        void AjouterAllergenesAuClient(int id, List<int> allergeneIds);

        /// <summary>
        /// Récupère l'historique des repas d'un client avec les menus complets.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des repas avec leurs menus triés par date décroissante.</returns>
        List<Repas> ObtenirHistoriqueRepas(int clientId);
    }
}
