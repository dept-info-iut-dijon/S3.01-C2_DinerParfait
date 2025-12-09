using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "Client";

        public ClientRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Client>> GetAllAsync()
        {
            List<Client>? clients = await _httpClient.GetFromJsonAsync<List<Client>>(BaseEndpoint);
            return clients ?? new List<Client>();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Client>($"{BaseEndpoint}/{id}");
        }

        public async Task<Client> CreateAsync(Client client)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, client);
            response.EnsureSuccessStatusCode();
            Client? created = await response.Content.ReadFromJsonAsync<Client>();
            return created ?? throw new System.InvalidOperationException("La création du client a échoué");
        }

        public async Task<bool> UpdateAsync(Client client)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{client.Id}", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Client>> GetAllWithAllergenesAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Client?> GetByIdWithAllergenesAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<List<Client>> SearchByNomAsync(string nom)
        {
            List<Client> allClients = await GetAllAsync();
            return allClients.Where(c => c.Nom.Contains(nom, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<bool> UpdateAllergenesAsync(int clientId, List<int> allergeneIds)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/{clientId}/allergenes", allergeneIds);
            return response.IsSuccessStatusCode;
        }

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
