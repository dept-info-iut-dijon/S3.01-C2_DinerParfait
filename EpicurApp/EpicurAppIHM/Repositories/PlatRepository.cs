using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    /// <summary>
    /// Repository responsable de la communication avec l'API pour les opérations sur les plats.
    /// </summary>
    public class PlatRepository : IPlatRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "api/plats";

        /// <summary>
        /// Initialise une nouvelle instance du repository.
        /// </summary>
        public PlatRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Récupère tous les plats depuis l'API.
        /// </summary>
        public async Task<List<Plat>> GetAllAsync()
        {
            List<Plat>? plats = await _httpClient.GetFromJsonAsync<List<Plat>>(BaseEndpoint);
            return plats ?? new List<Plat>();
        }

        /// <summary>
        /// Récupère un plat par son identifiant.
        /// </summary>
        public async Task<Plat?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Plat>($"{BaseEndpoint}/{id}");
        }

        /// <summary>
        /// Crée un nouveau plat via l'API.
        /// </summary>
        public async Task<Plat> CreateAsync(Plat plat)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, plat);
            response.EnsureSuccessStatusCode();
            Plat? created = await response.Content.ReadFromJsonAsync<Plat>();
            return created ?? throw new System.InvalidOperationException("La création du plat a échoué");
        }

        /// <summary>
        /// Met à jour un plat existant.
        /// </summary>
        public async Task<bool> UpdateAsync(Plat plat)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{plat.Id}", plat);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Supp un plat par son identifiant
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Récupère tous les plats avec leurs ingrédients associés
        /// </summary>
        public async Task<List<Plat>> GetAllWithIngredientsAsync()
        {
            return await GetAllAsync();
        }

        /// <summary>
        /// Récupère un plat par son identifiant avec ses ingrédients associés
        /// </summary>
        public async Task<Plat?> GetByIdWithIngredientsAsync(int id)
        {
            return await GetByIdAsync(id);
        }
    }
}
