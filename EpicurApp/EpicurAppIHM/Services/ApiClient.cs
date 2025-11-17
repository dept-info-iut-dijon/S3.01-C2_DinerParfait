using System;
using System.Net.Http;

namespace EpicurAppIHM.Services
{
    /// <summary>
    /// Classe pour gérer la connexion à l'API
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Obtient l'instance HttpClient
        /// </summary>
        public HttpClient HttpClient => _httpClient;

        /// <summary>
        /// Constructeur par défaut utilisant l'URL depuis la variable d'environnement ou l'URL par défaut
        /// </summary>
        public ApiClient() : this(GetDefaultBaseUrl())
        {
        }

        /// <summary>
        /// Constructeur avec URL personnalisée
        /// </summary>
        /// <param name="baseUrl">L'URL de base de l'API</param>
        public ApiClient(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("L'URL de base ne peut pas être vide", nameof(baseUrl));
            }

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient(handler, disposeHandler: true);
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        /// <summary>
        /// Obtient l'URL par défaut depuis la variable d'environnement ou retourne l'URL par défaut
        /// </summary>
        private static string GetDefaultBaseUrl()
        {
            string? baseUrl = Environment.GetEnvironmentVariable("EPICURAPP_API_BASEURL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "https://localhost:8081/";
            }
            return baseUrl;
        }
    }
}