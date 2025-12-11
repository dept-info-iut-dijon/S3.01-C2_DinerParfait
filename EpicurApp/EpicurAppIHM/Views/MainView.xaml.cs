using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cache toutes les vues
        /// </summary>
        private void CacherToutesLesVues()
        {
            DashboardViewControl.Visibility = Visibility.Collapsed;
            ClientsViewControl.Visibility = Visibility.Collapsed;
            MenusViewControl.Visibility = Visibility.Collapsed;
            BoiteIdeesViewControl.Visibility = Visibility.Collapsed;
            ReservationsViewControl.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Affiche la vue client
        /// </summary>
        private void AfficherClients(object sender, RoutedEventArgs e)
        {
            CacherToutesLesVues(); 
            ClientsViewControl.Visibility = Visibility.Visible; 
        }


        /// <summary>
        /// Affiche la vue menu
        /// </summary>
        private void AfficherMenus(object sender, RoutedEventArgs e)
        {
            CacherToutesLesVues();
            MenusViewControl.Visibility = Visibility.Visible;
            MenusViewControl.ChargerMenus();
        }


        /// <summary>
        /// affiche la vue boite a idee
        /// </summary>
        private void AfficherBoiteIdees(object sender, RoutedEventArgs e)
        {
            CacherToutesLesVues();
            BoiteIdeesViewControl.Visibility = Visibility.Visible;
            BoiteIdeesViewControl.ChargerIdees();
        }

        /// <summary>
        /// Affiche la vue reservation
        /// </summary>
        private void AfficherReservations(object sender, RoutedEventArgs e)
        {
            CacherToutesLesVues();
            ReservationsViewControl.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Affiche la vue dashboard
        /// </summary>
        public void AfficherDashboard()
        {
            CacherToutesLesVues(); 
            DashboardViewControl.Visibility = Visibility.Visible;
            DashboardViewControl.Rafraichir();
        }

        /// <summary>
        /// Affiche la vue etiquette
        /// </summary>
        private void OuvrirEtiquettes_Click(object sender, RoutedEventArgs e)
        {
            EtiquettesView etiquettesView = new EtiquettesView();
            etiquettesView.Show();
        }

        /// <summary>
        /// Affiche la vue acceuil
        /// </summary>
        private void OuvrirAcceuilClick(object sender, RoutedEventArgs e)
        {
            AfficherDashboard();
        }
    }
}