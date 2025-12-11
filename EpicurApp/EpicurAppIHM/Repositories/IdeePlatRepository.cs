using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    /// <summary>
    /// Repository responsable de la communication avec l'API pour les opérations sur les idées de plats
    /// </summary>
    public class IdeePlatRepository : IIdeePlatRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "IdeePlat";


        /// <summary>
        /// Initialise une nouvelle instance du repository
        /// </summary>
        public IdeePlatRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Récupère toutes les idées de plats depuis l'api
        /// </summary>
        public async Task<List<IdeePlat>> GetAllAsync()
        {
            List<IdeePlat>? idees = await _httpClient.GetFromJsonAsync<List<IdeePlat>>(BaseEndpoint);
            return idees ?? new List<IdeePlat>();
        }

        /// <summary>
        /// Récupère une idée de plat par son identifiant.
        /// </summary>
        public async Task<IdeePlat?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<IdeePlat>($"{BaseEndpoint}/{id}");
        }

        /// <summary>
        /// Crée une nouvelle idée de plat via l api
        /// </summary>
        public async Task<IdeePlat> CreateAsync(IdeePlat ideePlat)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, ideePlat);
            response.EnsureSuccessStatusCode();
            IdeePlat? created = await response.Content.ReadFromJsonAsync<IdeePlat>();
            return created ?? throw new System.InvalidOperationException("La création de l'idée a échoué");
        }

        /// <summary>
        /// met à jour une idée de plat existante
        /// </summary>
        public async Task<bool> UpdateAsync(IdeePlat ideePlat)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{ideePlat.Id}", ideePlat);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// supp une idée de plat par son identifiant.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
