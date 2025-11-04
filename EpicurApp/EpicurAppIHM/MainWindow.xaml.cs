using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EpicurAppIHM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Bouton Annuler il vide tous les champs
        private void Annuler(object sender, RoutedEventArgs e)
        {
            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            txtCarte.Clear();
            txtAllergies.Clear();
            txtPlats.Clear();
        }

        // Bouton Créer affiche message et vide
        private void CreerClient(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Client créé",
                           "Succès",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);

            Annuler(sender, e);
        }
    }
}