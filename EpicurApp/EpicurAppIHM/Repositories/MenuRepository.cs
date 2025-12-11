using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "menu";

        public MenuRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Menu>> GetAllAsync()
        {
            List<Menu>? menus = await _httpClient.GetFromJsonAsync<List<Menu>>(BaseEndpoint);
            return menus ?? new List<Menu>();
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Menu>($"{BaseEndpoint}/{id}");
        }

        public async Task<Menu> CreateAsync(Menu menu)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BaseEndpoint, menu);
            response.EnsureSuccessStatusCode();
            Menu? created = await response.Content.ReadFromJsonAsync<Menu>();
            return created ?? throw new System.InvalidOperationException("La création du menu a échoué");
        }

        public async Task<bool> UpdateAsync(Menu menu)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{menu.Id}", menu);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ElementListeCourse>> GetListeCoursesAsync(int menuId)
        {
            List<ElementListeCourse>? list = await _httpClient.GetFromJsonAsync<List<ElementListeCourse>>($"{BaseEndpoint}/{menuId}/listecourses");
            return list ?? new List<ElementListeCourse>();
        }

        public async Task<List<Menu>> GetAllWithPlatsAsync()
        {
            return await GetAllAsync();
        }

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

        public async Task<bool> AddNoteAsync(int menuId, int note)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseEndpoint}/{menuId}/AddNote", note);
            return response.IsSuccessStatusCode;
        }
    }
}
