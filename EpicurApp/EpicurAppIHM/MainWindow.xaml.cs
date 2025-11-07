using System.Windows;

namespace EpicurAppIHM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void FicheClient(object sender, RoutedEventArgs e)
        {
            FicheClient ficheClient = new FicheClient();
            ficheClient.ShowDialog();

            await ClientsViewControl.ChargerClients();
        }

        private void AffichageMenu(object sender, RoutedEventArgs e)
        {
            Views.MenusView menusView = new Views.MenusView();
            menusView.Show();
        }
    }
}
