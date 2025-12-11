using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    /// <summary>
    /// Repository responsable de la communication avec l'API pour les opérations CRUD sur les allergènes.
    /// </summary>
    public class AllergeneRepository : IAllergeneRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "Allergenes";

        public AllergeneRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Récupère tous les allergènes depuis l api
        /// </summary>
        public async Task<List<Allergene>> GetAllAsync()
        {
            List<Allergene>? list = await _httpClient.GetFromJsonAsync<List<Allergene>>(BaseEndpoint);
            return list ?? new List<Allergene>();
        }

        /// <summary>
        /// Récupère un allergène par son id
        /// </summary>
        public async Task<Allergene?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Allergene>($"{BaseEndpoint}/{id}");
        }

        /// <summary>
        /// Crée un nouvel allergène via l'api
        /// </summary>
        public async Task<Allergene> CreateAsync(Allergene allergene)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, allergene);
            response.EnsureSuccessStatusCode();
            Allergene? created = await response.Content.ReadFromJsonAsync<Allergene>();
            return created ?? throw new System.InvalidOperationException("La création de l'allergène a échoué");
        }

        /// <summary>
        /// met à jour un allergène existant
        /// </summary>
        public async Task<bool> UpdateAsync(Allergene allergene)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{allergene.Id}", allergene);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Supprime un allergène par son id
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
