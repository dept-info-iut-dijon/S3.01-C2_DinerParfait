using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Fenêtre principale de l'application
    /// </summary>
    public partial class MainView : UserControl
    {
        /// <summary>
        /// Construit la fenêtre principale
        /// </summary>
        public MainView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Affiche la vue des clients
        /// </summary>
        private void AfficherClients(object sender, RoutedEventArgs e)
        {
            ClientsViewControl.Visibility = Visibility.Visible;
            MenusViewControl.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Affiche la vue des menus
        /// </summary>
        private void AfficherMenus(object sender, RoutedEventArgs e)
        {
            ClientsViewControl.Visibility = Visibility.Collapsed;
            MenusViewControl.Visibility = Visibility.Visible;
       
            MenusViewControl.ChargerMenus();
        }
    }
}
