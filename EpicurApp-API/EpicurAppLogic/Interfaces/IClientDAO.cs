using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des clients.
    /// Fournit les opérations CRUD de base sur les clients.
    /// </summary>
    public interface IClientDAO
    {
        /// <summary>
        /// Récupère un client par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du client.</param>
        /// <returns>Le client trouvé.</returns>
        /// <exception cref="EpicurAPP_Partage.Exceptions.EntityNotFoundException">Si le client n'existe pas.</exception>
        Client GetById(int id);

        /// <summary>
        /// Récupère un client avec son historique complet de menus.
        /// </summary>
        /// <param name="id">Identifiant du client.</param>
        /// <returns>Le client avec son historique de menus.</returns>
        /// <exception cref="EpicurAPP_Partage.Exceptions.EntityNotFoundException">Si le client n'existe pas.</exception>
        Task<Client> GetByIdWithHistoryAsync(int id);

        /// <summary>
        /// Récupère tous les clients.
        /// </summary>
        /// <returns>Liste de tous les clients.</returns>
        List<Client> GetAll();

        /// <summary>
        /// Ajoute un nouveau client dans la base de données.
        /// </summary>
        /// <param name="client">Le client à ajouter.</param>
        void AjouterClient(Client client);

        /// <summary>
        /// Modifie un client existant.
        /// </summary>
        /// <param name="client">Le client avec les données mises à jour.</param>
        void ModifierClient(Client client);

        /// <summary>
        /// Supprime un client de la base de données.
        /// </summary>
        /// <param name="id">Identifiant du client à supprimer.</param>
        void SupprimerClient(int id);

        /// <summary>
        /// Associe des allergènes à un client.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="allergeneIds">Liste des identifiants d'allergènes à associer.</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);
        Client RechercherClientParId(int id);
    }
}

