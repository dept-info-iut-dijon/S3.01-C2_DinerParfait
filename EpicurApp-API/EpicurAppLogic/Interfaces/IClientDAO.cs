using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des clients
    /// </summary>
    public interface IClientDAO
    {
        /// <summary>
        /// Ajoute un client dans la base de données
        /// </summary>
        /// <param name="client">Le client à ajouter</param>
        void AjouterClient(Client client);

        /// <summary>
        /// Récupère un client par son identifiant
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns>Le client ou null si non trouvé</returns>
        Client? GetById(int id);

        /// <summary>
        /// Récupère tous les clients
        /// </summary>
        /// <returns>Liste de tous les clients</returns>
        List<Client> GetAll();

        /// <summary>
        /// Modifie un client existant
        /// </summary>
        /// <param name="client">Le client à modifier</param>
        void ModifierClient(Client client);

        /// <summary>
        /// Supprime un client
        /// </summary>
        /// <param name="id">Identifiant du client à supprimer</param>
        void SupprimerClient(int id);

        /// <summary>
        /// Recherche un client par son identifiant avec ses relations
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns>Le client avec ses allergènes et plats non appréciés</returns>
        Client? RechercherClientParId(int id);

        /// <summary>
        /// Récupère un client avec son historique de repas
        /// </summary>
        /// <param name="id">Identifiant du client</param>
        /// <returns>Le client avec son historique</returns>
        Task<Client> GetByIdWithHistoryAsync(int id);

        /// <summary>
        /// Associe des allergènes à un client
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);
    }
}
