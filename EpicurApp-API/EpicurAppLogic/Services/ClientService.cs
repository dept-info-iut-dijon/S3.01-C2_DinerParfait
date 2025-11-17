using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpicurAppLogic.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientDAO _clientRepository;

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
                List<Client> clients = _clientRepository.GetAll();
                return clients;
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
                Client? client = _clientRepository.RechercherClientParId(id);
                if (client == null)
                {
                    throw new Exception($"Client avec l'id {id} introuvable.");
                }
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
                Client? client = await _clientRepository.GetByIdWithHistoryAsync(id);
                if (client == null)
                {
                    throw new Exception($"Client avec l'id {id} introuvable.");
                }
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
    }
}
