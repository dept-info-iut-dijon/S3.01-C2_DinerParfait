using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    public class AllergeneRepository : IAllergeneRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "Allergene";

        public AllergeneRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Allergene>> GetAllAsync()
        {
            List<Allergene>? allergenes = await _httpClient.GetFromJsonAsync<List<Allergene>>(BaseEndpoint);
            return allergenes ?? new List<Allergene>();
        }

        public async Task<Allergene?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Allergene>($"{BaseEndpoint}/{id}");
        }

        public async Task<Allergene> CreateAsync(Allergene allergene)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, allergene);
            response.EnsureSuccessStatusCode();
            Allergene? created = await response.Content.ReadFromJsonAsync<Allergene>();
            return created ?? throw new System.InvalidOperationException("La création de l'allergène a échoué");
        }

        public async Task<bool> UpdateAsync(Allergene allergene)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{allergene.Id}", allergene);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
