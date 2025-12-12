using System;
using System.Net.Http;

namespace EpicurAppIHM.Services
{
    /// <summary>
    /// Client HTTP pour communiquer avec l'API.
    /// </summary>
    public class ApiClient
    {
        private HttpClient? _httpClient;

        /// <summary>
        /// Obtient l'instance HttpClient configurée pour communiquer avec l'API.
        /// </summary>
        public HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    // Détermination de l'URL de base
                    string baseUrl = Environment.GetEnvironmentVariable("EPICURAPP_API_BASEURL")
                        ?? "https://10.128.207.45:9443/";

                    // Configuration du handler pour accepter tous les certificats SSL
                    HttpClientHandler handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                    };

                    // Création et configuration du HttpClient
                    _httpClient = new HttpClient(handler, disposeHandler: true)
                    {
                        BaseAddress = new Uri(baseUrl)
                    };
                }
                return _httpClient;
            }
        }

        /// <summary>
        /// Définit le RestaurantId pour toutes les requêtes futures via le header X-Restaurant-Id.
        /// </summary>
        /// <param name="restaurantId">L'ID du restaurant</param>
        public void SetRestaurantId(int restaurantId)
        {
            if (HttpClient.DefaultRequestHeaders.Contains("X-Restaurant-Id"))
            {
                HttpClient.DefaultRequestHeaders.Remove("X-Restaurant-Id");
            }
            HttpClient.DefaultRequestHeaders.Add("X-Restaurant-Id", restaurantId.ToString());
        }

        /// <summary>
        /// Supprime le RestaurantId des headers (utilisé à la déconnexion).
        /// </summary>
        public void ClearRestaurantId()
        {
            if (HttpClient.DefaultRequestHeaders.Contains("X-Restaurant-Id"))
            {
                HttpClient.DefaultRequestHeaders.Remove("X-Restaurant-Id");
            }
        }
    }
}