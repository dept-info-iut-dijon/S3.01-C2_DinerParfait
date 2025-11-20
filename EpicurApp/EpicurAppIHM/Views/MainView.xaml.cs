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

        /// <summary>
        /// Affiche la vue des étiquettes
        /// </summary>
        private void OuvrirEtiquettes_Click(object sender, RoutedEventArgs e)
        {
            // Créer une nouvelle instance de la fenêtre
            var fenetreEtiquettes = new EtiquettesView();

            //L'afficher
            fenetreEtiquettes.ShowDialog();
        }
    }
}
