using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    public class IdeePlatRepository : IIdeePlatRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "IdeePlat";

        public IdeePlatRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<IdeePlat>> GetAllAsync()
        {
            List<IdeePlat>? idees = await _httpClient.GetFromJsonAsync<List<IdeePlat>>(BaseEndpoint);
            return idees ?? new List<IdeePlat>();
        }

        public async Task<IdeePlat?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<IdeePlat>($"{BaseEndpoint}/{id}");
        }

        public async Task<IdeePlat> CreateAsync(IdeePlat ideePlat)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, ideePlat);
            response.EnsureSuccessStatusCode();
            IdeePlat? created = await response.Content.ReadFromJsonAsync<IdeePlat>();
            return created ?? throw new System.InvalidOperationException("La création de l'idée a échoué");
        }

        public async Task<bool> UpdateAsync(IdeePlat ideePlat)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{ideePlat.Id}", ideePlat);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
