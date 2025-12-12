using System.Windows;
using EpicurAppIHM.Services;
using EpicurAppIHM.Repositories;
using EpicurAppIHM.RepositoriesIntefaces;
using EpicurAppIHM.Views;

namespace EpicurAppIHM
{
    public partial class App : Application
    {
        /// <summary>
        /// Instance partagée de l'ApiClient pour toute l'application
        /// </summary>
        public static ApiClient ApiClient { get; } = new ApiClient();

        /// <summary>
        /// Utilisateur actuellement connecté
        /// </summary>
        public static UtilisateurInfo? CurrentUser { get; set; }

        /// <summary>
        /// Restaurant de l'utilisateur connecté
        /// </summary>
        public static RestaurantInfo? CurrentRestaurant { get; set; }

        /// <summary>
        ///Couche d'abstraction pour l'accès aux données
        /// </summary>
        public static IMenuRepository MenuRepository { get; } = new MenuRepository(ApiClient.HttpClient);
        public static IPlatRepository PlatRepository { get; } = new PlatRepository(ApiClient.HttpClient);
        public static IClientRepository ClientRepository { get; } = new ClientRepository(ApiClient.HttpClient);
        public static IIdeePlatRepository IdeePlatRepository { get; } = new IdeePlatRepository(ApiClient.HttpClient);
        public static IAllergeneRepository AllergeneRepository { get; } = new AllergeneRepository(ApiClient.HttpClient);
        public static IAllergeneDetectionRepository AllergeneDetectionRepository { get; } = new AllergeneDetectionRepository(ApiClient.HttpClient);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Gestion globale des exceptions non gérées
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                MessageBox.Show($"Une erreur critique est survenue (Unhandled) :\n{ex.Message}\n\n{ex.StackTrace}", 
                                "Erreur Critique EpicurApp", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
            };

            try
            {
                // Tente d'instancier et d'afficher la vue de connexion
                var loginView = new Views.LoginView();
                loginView.Show();
            }
            catch (Exception ex)
            {
                // Affiche l'erreur si le lancement échoue (ex: ressource introuvable)
                MessageBox.Show($"Impossible de démarrer l'application :\n{ex.Message}\n\n{ex.InnerException?.Message}", 
                                "Erreur de Démarrage EpicurApp", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
