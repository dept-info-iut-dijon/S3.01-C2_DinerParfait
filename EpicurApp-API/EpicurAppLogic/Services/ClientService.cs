using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    public class ClientService : IClientService
    {
        private IClientDAO _clientRepository;
        private IRepasDAO _repasRepository;

        public ClientService(IClientDAO clientRepository, IRepasDAO repasRepository)
        {
            _clientRepository = clientRepository;
            _repasRepository = repasRepository;
        }

        public void AjouterClient(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Nom) || string.IsNullOrWhiteSpace(client.Prenom))
            {
                throw new InvalidFieldException("Le nom et le prenom sont obligatoires.");
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

        public List<Repas> ObtenirHistoriqueRepas(int clientId)
        {
            try
            {
                return _repasRepository.GetRepasByClientId(clientId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erreur lors de la récupération de l'historique des repas pour le client {clientId}.", ex);
            }
        }

    }
}