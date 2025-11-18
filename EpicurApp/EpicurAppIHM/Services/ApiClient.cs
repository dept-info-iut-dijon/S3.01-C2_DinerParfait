using System;
using System.Net.Http;

namespace EpicurAppIHM.Services
{
    /// <summary>
    /// Classe singleton pour gérer l'instance HttpClient 
    /// </summary>
    public static class ApiClient
    {

        private static  HttpClient _instance;

        public static HttpClient Instance
        {
            get { return _instance; }
        }

        static ApiClient()
        {
            string? baseUrl = Environment.GetEnvironmentVariable("EPICURAPP_API_BASEURL");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                //baseUrl = "https://10.128.207.45:8081/";
                baseUrl = "https://localhost:7068/";
            }

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

            _instance = new HttpClient(handler, disposeHandler: true);
            _instance.BaseAddress = new Uri(baseUrl);
        }
    }
}