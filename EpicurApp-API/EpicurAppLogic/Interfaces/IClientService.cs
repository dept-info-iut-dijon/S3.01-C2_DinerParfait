using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations du service liées à la gestion des clients.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Ajoute un nouveau client dans la base de données.
        /// </summary>
        /// <param name="client">L'objet <see cref="Client"/> contenant les informations du client à ajouter.</param>
        void AjouterClient(Client client);

        /// <summary>
        /// Récupère tous les clients.
        /// </summary>
        /// <returns>Liste de tous les clients.</returns>
        List<Client> ObtenirTousLesClients();

        /// <summary>
        /// Récupère un client par son ID.
        /// </summary>
        /// <param name="id">L'ID du client.</param>
        /// <returns>Le client correspondant.</returns>
        Client ObtenirClientParId(int id);

        /// <summary>
        /// Récupère un client avec son historique de manière asynchrone.
        /// </summary>
        /// <param name="id">L'ID du client.</param>
        /// <returns>Le client avec son historique.</returns>
        Task<Client> ObtenirClientAvecHistoriqueAsync(int id);

        /// <summary>
        /// Ajoute des allergènes à un client.
        /// </summary>
        /// <param name="id">L'ID du client.</param>
        /// <param name="allergeneIds">Liste des IDs d'allergènes.</param>
        void AjouterAllergenesAuClient(int id, List<int> allergeneIds);

        /// <summary>
        /// Récupère l'historique des repas d'un client avec les menus complets (US 1.5).
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des repas avec leurs menus triés par date décroissante.</returns>
        List<Repas> ObtenirHistoriqueRepas(int clientId);

        /// <summary>
        /// Modifie un client existant (US 1.3).
        /// </summary>
        /// <param name="client">L'objet <see cref="Client"/> contenant les informations modifiées.</param>
        void ModifierClient(Client client);

        /// <summary>
        /// Supprime un client en fonction de son ID (US 1.4).
        /// </summary>
        /// <param name="id">L'ID du client à supprimer.</param>
        void Delete(int id);

        /// <summary>
        /// Récupère les clients réguliers (3 visites ou plus sur l'année).
        /// </summary>
        /// <returns>Liste des clients réguliers.</returns>
        List<Client> ObtenirClientsReguliers();

        /// <summary>
        /// Récupère les clients inactifs (pas de visite depuis plus de 60 jours).
        /// </summary>
        /// <returns>Liste des clients inactifs.</returns>
        List<Client> ObtenirClientsInactifs();


        /// <summary>
        /// Récupère les clients vip
        /// </summary>
        List<Client> ObtenirClientsVIP();
    }
}