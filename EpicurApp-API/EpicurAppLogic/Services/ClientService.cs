using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    public class ClientService : IClientService
    {
        private IClientDAO _clientRepository;

        public ClientService(IClientDAO clientRepository)
        {
            _clientRepository = clientRepository;
        }

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

        public Client ObtenirClientParId(int id)
        {
            try
            {
                var client = _clientRepository.RechercherClientParId(id);
                if (client == null)
                    throw new Exception($"Client avec l'id {id} introuvable.");
                return client;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du client.", ex);
            }
        }


        public async Task<Client> ObtenirClientAvecHistoriqueAsync(int id)
        {
            try
            {
                var client = await _clientRepository.GetByIdWithHistoryAsync(id);
                if (client == null)
                    throw new Exception($"Client avec l'id {id} introuvable.");
                return client;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du client avec historique.", ex);
            }
        }

        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            _clientRepository.AjouterAllergenesAuClient(clientId, allergeneIds);
        }


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
                var ancienClient = _clientRepository.RechercherClientParId(client.Id);

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
                string logPath = "modifications_clients.log";
                string champsStr = champsModifies.Count > 0 ? string.Join(", ", champsModifies) : "Aucun";
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Modification - Client ID {client.Id} - {client.Prenom} {client.Nom} - {champsStr}\n";
                System.IO.File.AppendAllText(logPath, logEntry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la modification du client.", ex);
            }
        }



    }

    
}