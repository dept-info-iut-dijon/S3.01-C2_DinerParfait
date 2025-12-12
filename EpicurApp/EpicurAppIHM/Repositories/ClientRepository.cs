using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    /// <summary>
    /// Repository responsable de la communication avec l'API pour les opérations sur les clients.
    /// </summary>
    public class ClientRepository : IClientRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "Client";


        /// <summary>
        /// Initialise une nouvelle instance du repository.
        /// </summary>
        public ClientRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Récupère tous les clients depuis l'API.
        /// </summary>
        public async Task<List<Client>> GetAllAsync()
        {
            List<Client>? clients = await _httpClient.GetFromJsonAsync<List<Client>>(BaseEndpoint);
            return clients ?? new List<Client>();
        }

        /// <summary>
        /// Récupère un client par son identifiant.
        /// </summary>
        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Client>($"{BaseEndpoint}/{id}");
        }

        /// <summary>
        /// Crée un nouveau client via l'api
        /// </summary>
        public async Task<Client> CreateAsync(Client client)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, client);
            response.EnsureSuccessStatusCode();
            Client? created = await response.Content.ReadFromJsonAsync<Client>();
            return created ?? throw new System.InvalidOperationException("La création du client a échoué");
        }

        /// <summary>
        /// Met à jour un client existant.
        /// </summary>
        public async Task<bool> UpdateAsync(Client client)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{client.Id}", client);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Supprime un client par son identifiant.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Recherche des clients par leur nom.
        /// </summary>
        public async Task<List<Client>> SearchByNomAsync(string nom)
        {
            List<Client> allClients = await GetAllAsync();
            return allClients.Where(c => c.Nom.Contains(nom, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Met à jour les allergènes associés à un client.
        /// </summary>
        public async Task<bool> UpdateAllergenesAsync(int clientId, List<int> allergeneIds)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/{clientId}/allergenes", allergeneIds);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Récupère tous les repas d'un client.
        /// </summary>
        public async Task<List<Repas>> GetRepasAsync(int clientId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{BaseEndpoint}/{clientId}/repas");

            if (!response.IsSuccessStatusCode)
                return new List<Repas>();

            string content = await response.Content.ReadAsStringAsync();
            if (content.Contains("Aucun repas enregistré"))
                return new List<Repas>();

            List<Repas>? repas = await response.Content.ReadFromJsonAsync<List<Repas>>();
            return repas ?? new List<Repas>();
        }
    }
}
