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
            DashboardViewControl.Visibility = Visibility.Collapsed;
            ClientsViewControl.Visibility = Visibility.Visible;
            MenusViewControl.Visibility = Visibility.Collapsed;
            BoiteIdeesViewControl.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Affiche la vue des menus
        /// </summary>
        private void AfficherMenus(object sender, RoutedEventArgs e)
        {
            DashboardViewControl.Visibility = Visibility.Collapsed;
            ClientsViewControl.Visibility = Visibility.Collapsed;
            MenusViewControl.Visibility = Visibility.Visible;
            BoiteIdeesViewControl.Visibility = Visibility.Collapsed;

            MenusViewControl.ChargerMenus();
        }

        /// <summary>
        /// Affiche la vue  boite a idees
        /// </summary>
        private void AfficherBoiteIdees(object sender, RoutedEventArgs e)
        {
            DashboardViewControl.Visibility = Visibility.Collapsed;
            ClientsViewControl.Visibility = Visibility.Collapsed;
            MenusViewControl.Visibility = Visibility.Collapsed;
            BoiteIdeesViewControl.Visibility = Visibility.Visible;

            BoiteIdeesViewControl.ChargerIdees();
        }

    
        /// <summary>
        /// Affiche le Dashboard page d'accueil
        /// </summary>
        public void AfficherDashboard()
        {
            DashboardViewControl.Visibility = Visibility.Visible;
            ClientsViewControl.Visibility = Visibility.Collapsed;
            MenusViewControl.Visibility = Visibility.Collapsed;
            BoiteIdeesViewControl.Visibility = Visibility.Collapsed;

            DashboardViewControl.Rafraichir();
        }

        /// <summary>
        /// Ouvre la fenêtre des étiquettes
        /// </summary>
        private void OuvrirEtiquettes_Click(object sender, RoutedEventArgs e)
        {
            EtiquettesView etiquettesView = new EtiquettesView();
            etiquettesView.Show();
        }
    }
}
