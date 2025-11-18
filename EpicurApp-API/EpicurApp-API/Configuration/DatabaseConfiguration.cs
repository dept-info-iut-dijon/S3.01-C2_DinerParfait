using Microsoft.Extensions.Configuration;

namespace EpicurApp_API.Configuration
{
    /// <summary>
    /// Classe de configuration centralisée pour la base de données.
    /// </summary>
    public class DatabaseConfiguration
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialise une nouvelle instance de DatabaseConfiguration.
        /// </summary>
        /// <param name="configuration">Configuration de l'application.</param>
        public DatabaseConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Obtient la chaîne de connexion à la base de données.
        /// </summary>
        /// <returns>La chaîne de connexion configurée ou la valeur par défaut.</returns>
        public string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=epicurapp.db";
        }
    }
}
