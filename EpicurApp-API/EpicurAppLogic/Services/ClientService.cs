using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using EpicurApp_API.Models;

namespace EpicurApp.Logic.Services
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
    }
}
