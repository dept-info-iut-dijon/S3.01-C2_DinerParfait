using System.Data.SQLite;
using EpicurAppData;
using EpicurAppData.Repositories;
using EpicurAppLogic.Exceptions;
using EpicurAppLogic.Services;
using Microsoft.Data.Sqlite;

namespace EpicurApp.Logic.Services
{
    public class ClientService : IClientService
    {
        private IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
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
    }
}
