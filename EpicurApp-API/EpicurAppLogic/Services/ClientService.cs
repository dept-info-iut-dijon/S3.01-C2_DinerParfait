using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Le service des clients
    /// </summary>
    public class ClientService : IClientService
    {
        private IClientDAO _clientRepository;

        /// <summary>
        /// Constructeur de la classe ClientService
        /// </summary>
        /// <param name="clientRepository">Le DAO pour interagir avec le client</param>
        public ClientService(IClientDAO clientRepository)
        {
            _clientRepository = clientRepository;
        }

        /// <summary>
        /// Méthode pour ajouter un client
        /// </summary>
        /// <param name="client">client a ajouter</param>
        /// <exception cref="InvalidFieldException">Le nom et prénom ne peuvent être nul</exception>
        /// <exception cref="ApplicationException">Impossible d'ajouter le client</exception>
        public void AjouterClient(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Nom) || string.IsNullOrWhiteSpace(client.Prenom))
            {
                throw new InvalidFieldException("Le nom et le prénom sont obligatoires.");
            }

            try
            {
                _clientRepository.AjouterClient(client);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'enregistrement du client.", ex);
            }
        }

        /// <summary>
        /// Liste de tous les clients
        /// </summary>
        /// <returns>La liste de tout les clients</returns>
        /// <exception cref="ApplicationException">Impossible de recuperer la liste des clients</exception>
        public List<Client> ObtenirTousLesClients()
        {
            try
            {
                return _clientRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des clients.", ex);
            }
        }

        /// <summary>
        /// Méthode pour obtenir un client par son id
        /// </summary>
        /// <param name="id">id du cient cherché</param>
        /// <returns>le client avec l'id correspondant</returns>
        /// <exception cref="ApplicationException">Impossible de recuperer le client</exception>
        public Client ObtenirClientParId(int id)
        {
            try
            {
                Client client = _clientRepository.RechercherClientParId(id);
                if (client == null)
                    throw new Exception($"Client avec l'id {id} introuvable.");
                return client;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du client.", ex);
            }
        }

        /// <summary>
        /// Méthode pour obtenir un client avec son historique par son id
        /// </summary>
        /// <param name="id">id du client dont on veut l'historique</param>
        /// <returns>l'historique du clien</returns>
        /// <exception cref="ApplicationException">Impossible de recuperer l'historique du client</exception>
        public async Task<Client> ObtenirClientAvecHistoriqueAsync(int id)
        {
            try
            {
                Client client = await _clientRepository.GetByIdWithHistoryAsync(id);
                if (client == null)
                    throw new Exception($"Client avec l'id {id} introuvable.");
                return client;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du client avec historique.", ex);
            }
        }

        /// <summary>
        /// Ajoute des allergenes a un client
        /// </summary>
        /// <param name="clientId">id du client allergique</param>
        /// <param name="allergeneIds">id des allergies du client</param>
        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            _clientRepository.AjouterAllergenesAuClient(clientId, allergeneIds);
        }

        /// <summary>
        /// Modifie un client
        /// </summary>
        /// <param name="client">client a modifier</param>
        /// <exception cref="InvalidFieldException">nom, prénom, email  doiven tre remplis</exception>
        /// <exception cref="ApplicationException">Impossible de modifier le client</exception>
        public void ModifierClient(Client client)
        {
            // Validation des champs obligatoires
            if (string.IsNullOrWhiteSpace(client.Nom) ||
                string.IsNullOrWhiteSpace(client.Prenom) ||
                string.IsNullOrWhiteSpace(client.Email) ||
                string.IsNullOrWhiteSpace(client.Telephone))
            {
                throw new InvalidFieldException("Le nom, prénom, email et téléphone sont obligatoires.");
            }

            try
            {
                // Récupérer l'ancien client pour comparer
                Client ancienClient = _clientRepository.RechercherClientParId(client.Id);

                // Trouver les champs modifiés avec anciennes et nouvelles valeurs
                List<string> champsModifies = new List<string>();

                if (ancienClient.Nom != client.Nom)
                    champsModifies.Add($"Nom: {ancienClient.Nom} = {client.Nom}");
                if (ancienClient.Prenom != client.Prenom)
                    champsModifies.Add($"Prenom: {ancienClient.Prenom} = {client.Prenom}");
                if (ancienClient.Email != client.Email)
                    champsModifies.Add($"Email: {ancienClient.Email} = {client.Email}");
                if (ancienClient.Telephone != client.Telephone)
                    champsModifies.Add($"Telephone: {ancienClient.Telephone} = {client.Telephone}");
                //if (ancienClient.PlatsNonApprecies != client.PlatsNonApprecies)
                // champsModifies.Add($"PlatsNonApprecies: {ancienClient.PlatsNonApprecies} = {client.PlatsNonApprecies}");
                if (ancienClient.Preferences != client.Preferences)
                    champsModifies.Add($"Preferences: {ancienClient.Preferences} = {client.Preferences}");

                // Mise à jour en base de données
                _clientRepository.ModifierClient(client);

                // Log de la modification dans un fichier texte
                string logPath = "LogsClients.log";
                string champsStr = champsModifies.Count > 0 ? string.Join(", ", champsModifies) : "Aucun";
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Modification - Client ID {client.Id} - {client.Prenom} {client.Nom} - {champsStr}\n";
                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la modification du client.", ex);
            }
        }

        /// <summary>
        /// Supprime un client par son id
        /// </summary>
        /// <param name="id">id du client a supprimer</param>
        /// <exception cref="ApplicationException">Impossible de supprimer le client</exception>
        public void Delete(int id)
        {
            try
            {
                // Récupérer les informations du client avant suppression pour le log
                Client clientASupprimer = _clientRepository.RechercherClientParId(id);
                
                if (clientASupprimer == null)
                {
                    throw new Exception($"Client avec l'id {id} introuvable.");
                }

                // Appelle la méthode Delete du DAO
                _clientRepository.SupprimerClient(id);

                // Log de la suppression dans le fichier LogsClients.log
                string logPath = "LogsClients.log";
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Suppression - Client ID {clientASupprimer.Id}\n";
                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erreur service : {ex.Message}", ex);
            }
        }
    }  
}