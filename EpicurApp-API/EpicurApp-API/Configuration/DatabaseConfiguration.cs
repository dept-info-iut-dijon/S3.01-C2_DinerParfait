using Microsoft.Data.Sqlite;

namespace EpicurApp_API.Configuration
{
    /// <summary>
    /// Classe de configuration centralisée pour la connexion à la base de données
    /// </summary>
    public class DatabaseConfiguration
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructeur initialisant la chaîne de connexion depuis la configuration
        /// </summary>
        /// <param name="configuration">Configuration de l'application</param>
        public DatabaseConfiguration(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=epicurapp.db";
        }

        /// <summary>
        /// Obtient la chaîne de connexion à la base de données
        /// </summary>
        public string ConnectionString => _connectionString;

        /// <summary>
        /// Crée et retourne une nouvelle connexion SQLite
        /// </summary>
        /// <returns>Nouvelle instance de SqliteConnection</returns>
        public SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
