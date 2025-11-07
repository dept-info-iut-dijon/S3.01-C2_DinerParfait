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
                baseUrl = "http://localhost:8080/";
            }

            _instance = new HttpClient();
            _instance.BaseAddress = new Uri(baseUrl);
        }
    }
}