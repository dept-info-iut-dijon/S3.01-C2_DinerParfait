using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    public class PlatRepository : IPlatRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "Plat";

        public PlatRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Plat>> GetAllAsync()
        {
            List<Plat>? plats = await _httpClient.GetFromJsonAsync<List<Plat>>(BaseEndpoint);
            return plats ?? new List<Plat>();
        }

        public async Task<Plat?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Plat>($"{BaseEndpoint}/{id}");
        }

        public async Task<Plat> CreateAsync(Plat plat)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, plat);
            response.EnsureSuccessStatusCode();
            Plat? created = await response.Content.ReadFromJsonAsync<Plat>();
            return created ?? throw new System.InvalidOperationException("La création du plat a échoué");
        }

        public async Task<bool> UpdateAsync(Plat plat)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{plat.Id}", plat);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Plat>> GetAllWithIngredientsAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Plat?> GetByIdWithIngredientsAsync(int id)
        {
            return await GetByIdAsync(id);
        }
    }
}
