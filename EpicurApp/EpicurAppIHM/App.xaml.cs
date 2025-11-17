using System.Configuration;
using System.Data;
using System.Windows;
using EpicurAppIHM.Services;

namespace EpicurAppIHM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Instance partagée de l'ApiClient pour toute l'application
        /// </summary>
        public static ApiClient ApiClient { get; } = new ApiClient();
    }

}
