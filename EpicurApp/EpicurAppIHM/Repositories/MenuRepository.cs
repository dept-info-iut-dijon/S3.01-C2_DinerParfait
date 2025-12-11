using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{

    /// <summary>
    /// Repository responsable de la communication avec l'API pour les opérations sur les menus.
    /// </summary>
    public class MenuRepository : IMenuRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "menu";

        /// <summary>
        /// Initialise une nouvelle instance du repository.
        /// </summary>
        public MenuRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Récupère tous les menus depuis l'API.
        /// </summary>
        public async Task<List<Menu>> GetAllAsync()
        {
            List<Menu>? menus = await _httpClient.GetFromJsonAsync<List<Menu>>(BaseEndpoint);
            return menus ?? new List<Menu>();
        }

        /// <summary>
        /// Récupère un menu par son identifiant.
        /// </summary>
        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Menu>($"{BaseEndpoint}/{id}");
        }

        /// <summary>
        /// Crée un nouveau menu via l'API.
        /// </summary>
        public async Task<Menu> CreateAsync(Menu menu)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, menu);
            response.EnsureSuccessStatusCode();
            Menu? created = await response.Content.ReadFromJsonAsync<Menu>();
            return created ?? throw new System.InvalidOperationException("La création du menu a échoué");
        }

        /// <summary>
        /// Met à jour un menu existant.
        /// </summary>
        public async Task<bool> UpdateAsync(Menu menu)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{menu.Id}", menu);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Supprime un menu par son identifiant.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Récupère la liste de courses générée pour un menu.
        /// </summary>
        public async Task<List<ElementListeCourse>> GetListeCoursesAsync(int menuId)
        {
            List<ElementListeCourse>? list = await _httpClient.GetFromJsonAsync<List<ElementListeCourse>>($"{BaseEndpoint}/{menuId}/listecourses");
            return list ?? new List<ElementListeCourse>();
        }

        /// <summary>
        /// Récupère tous les menus avec leurs plats associés.
        /// </summary>
        /// <returns>Liste des menus avec leurs plats</returns>
        public async Task<List<Menu>> GetAllWithPlatsAsync()
        {
            return await GetAllAsync();
        }

        /// <summary>
        /// Récupère le menu brouillon en cours d'édition.
        /// </summary>
        public async Task<Menu?> GetBrouillonAsync()
        {
            // Récupérer le menu brouillon directement depuis l'API
            return await _httpClient.GetFromJsonAsync<Menu>("menu/brouillon");
        }

        public async Task<List<Menu>> GetMenusValidesAsync()
        {
            // Récupérer uniquement les menus validés (disponibles pour les services)
            List<Menu>? menus = await _httpClient.GetFromJsonAsync<List<Menu>>("menu/valides");
            return menus ?? new List<Menu>();
        }

        // <summary>
        /// Ajoute une note à un menu
        /// </summary>
        public async Task<bool> AddNoteAsync(int menuId, int note)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/{menuId}/AddNote", note);
            return response.IsSuccessStatusCode;
        }
    }
}
