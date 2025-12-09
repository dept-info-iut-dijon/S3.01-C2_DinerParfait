using System.Windows;
using EpicurAppIHM.Services;
using EpicurAppIHM.Repositories;
using EpicurAppIHM.RepositoriesIntefaces;

namespace EpicurAppIHM
{
    public partial class App : Application
    {
        /// <summary>
        /// Instance partagée de l'ApiClient pour toute l'application
        /// </summary>
        public static ApiClient ApiClient { get; } = new ApiClient();

        /// <summary>
        ///Couche d'abstraction pour l'accès aux données
        /// </summary>
        public static IMenuRepository MenuRepository { get; } = new MenuRepository(ApiClient.HttpClient);
        public static IPlatRepository PlatRepository { get; } = new PlatRepository(ApiClient.HttpClient);
        public static IClientRepository ClientRepository { get; } = new ClientRepository(ApiClient.HttpClient);
        public static IAllergeneRepository AllergeneRepository { get; } = new AllergeneRepository(ApiClient.HttpClient);
        public static IIdeePlatRepository IdeePlatRepository { get; } = new IdeePlatRepository(ApiClient.HttpClient);
    }

}
