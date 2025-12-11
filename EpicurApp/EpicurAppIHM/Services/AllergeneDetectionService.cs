using EpicurAPP_Partage.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace EpicurAppIHM.Services
{
    /// <summary>
    /// Service pour la détection des conflits d'allergènes
    /// </summary>
    public class AllergeneDetectionService
    {
        private readonly HttpClient _httpClient;

        public AllergeneDetectionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Détecte les conflits pour un client et un menu
        /// </summary>
        public async Task<ValidationReservationResponse?> DetecterConflitAsync(int clientId, int menuId)
        {
            try
            {
                // Utiliser GetAsync pour récupérer la réponse même en cas d'erreur HTTP
                var response = await _httpClient.GetAsync($"AllergeneDetection/detecter/{clientId}/{menuId}");
                
                // Parser le JSON quelle que soit le code HTTP (200, 400, 409...)
                var content = await response.Content.ReadAsStringAsync();
                
                if (!string.IsNullOrEmpty(content))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<ValidationReservationResponse>(
                        content, 
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}