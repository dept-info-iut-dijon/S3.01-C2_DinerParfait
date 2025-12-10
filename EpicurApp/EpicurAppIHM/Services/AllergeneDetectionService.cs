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
        public async Task<ConflitAllergene?> DetecterConflitAsync(int clientId, int menuId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ConflitAllergene>(
                    $"AllergeneDetection/detecter/{clientId}/{menuId}");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Détecte les conflits pour plusieurs clients
        /// </summary>
        public async Task<List<ConflitAllergene>?> DetecterConflitsMultiplesAsync(List<int> clientIds, int menuId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"AllergeneDetection/detecter-multiple/{menuId}", clientIds);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ConflitAllergene>>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Valide une réservation avec possibilité d'override
        /// </summary>
        public async Task<bool> ValiderReservationAsync(ValidationReservationResponse request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "AllergeneDetection/valider-reservation", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}