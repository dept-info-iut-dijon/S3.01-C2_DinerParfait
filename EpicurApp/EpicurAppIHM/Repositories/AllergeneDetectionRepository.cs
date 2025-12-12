using EpicurAPP_Partage.Models;
using EpicurAppIHM.RepositoriesIntefaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EpicurAppIHM.Repositories
{
    /// <summary>
    /// Repository pour la détection des conflits d'allergènes
    /// </summary>
    public class AllergeneDetectionRepository : IAllergeneDetectionRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseEndpoint = "AllergeneDetection";

        public AllergeneDetectionRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Détecte les conflits pour un client et un menu
        /// </summary>
        public async Task<ValidationReservationResponse?> DetecterConflitAsync(int clientId, int menuId)
        {
            try
            {
                // Utiliser GetAsync pour récupérer la réponse même en cas d'erreur HTTP
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/detecter/{clientId}/{menuId}");

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
