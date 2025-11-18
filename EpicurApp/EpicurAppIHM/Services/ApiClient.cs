using System;
using System.Net.Http;

namespace EpicurAppIHM.Services
{
    /// <summary>
    /// Client HTTP simple pour communiquer avec l'API EpicurApp.
    /// </summary>
    public static class ApiClient
    {
        private static HttpClient? _httpClient;

        /// <summary>
        /// Obtient l'instance HttpClient configurée pour communiquer avec l'API.
        /// </summary>
        public static HttpClient Instance
        {
            get
            {
                if (_httpClient == null)
                {
                    // Détermination de l'URL de base
                    string baseUrl = Environment.GetEnvironmentVariable("EPICURAPP_API_BASEURL")
                        ?? "https://localhost:8081/";

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
    }
}