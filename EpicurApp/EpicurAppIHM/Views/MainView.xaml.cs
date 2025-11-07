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

        private void AfficherClients(object sender, RoutedEventArgs e)
        {
            ClientsViewControl.Visibility = Visibility.Visible;
            MenusViewControl.Visibility = Visibility.Collapsed;
        }

        private void AfficherMenus(object sender, RoutedEventArgs e)
        {
            ClientsViewControl.Visibility = Visibility.Collapsed;
            MenusViewControl.Visibility = Visibility.Visible;

       
            MenusViewControl.ChargerMenus();
        }
    }
}
