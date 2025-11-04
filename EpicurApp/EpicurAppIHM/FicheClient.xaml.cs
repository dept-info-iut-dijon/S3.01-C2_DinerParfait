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
    /// Interaction logic for FicheClient.xaml
    /// </summary>
    public partial class FicheClient : Window
    {
        public FicheClient()
        {
            InitializeComponent();
        }



        // Valide le prénom
        private void ValiderPrenom(object sender, RoutedEventArgs e)
        {
            string prenom = txtPrenom.Text.Trim();

            if (!string.IsNullOrEmpty(prenom) && !Regex.IsMatch(prenom, @"^[a-zA-ZÀ-ÿ\s'-]+$"))
            {
                txtPrenom.BorderBrush = Brushes.Red;
                erreurPrenom.Visibility = Visibility.Visible;
            }
            else
            {
                txtPrenom.BorderBrush = Brushes.Gray;
                erreurPrenom.Visibility = Visibility.Collapsed;
            }
        }

        // Valide le nom
        private void ValiderNom(object sender, RoutedEventArgs e)
        {
            string nom = txtNom.Text.Trim();

            if (!string.IsNullOrEmpty(nom) && !Regex.IsMatch(nom, @"^[a-zA-ZÀ-ÿ\s'-]+$"))
            {
                txtNom.BorderBrush = Brushes.Red;
                erreurNom.Visibility = Visibility.Visible;
            }
            else
            {
                txtNom.BorderBrush = Brushes.Gray;
                erreurNom.Visibility = Visibility.Collapsed;
            }
        }

        // Valide l'email
        private void ValiderEmail(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                txtEmail.BorderBrush = Brushes.Red;
                erreurEmail.Visibility = Visibility.Visible;
            }
            else
            {
                txtEmail.BorderBrush = Brushes.Gray;
                erreurEmail.Visibility = Visibility.Collapsed;
            }
        }

        // Valide le téléphone
        private void ValiderTelephone(object sender, RoutedEventArgs e)
        {
            string tel = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", "");

            if (!string.IsNullOrEmpty(tel) && !Regex.IsMatch(tel, @"^0[1-9]\d{8}$"))
            {
                txtTelephone.BorderBrush = Brushes.Red;
                erreurTelephone.Visibility = Visibility.Visible;
            }
            else
            {
                txtTelephone.BorderBrush = Brushes.Gray;
                erreurTelephone.Visibility = Visibility.Collapsed;
            }
        }

        

        // Vérifie tout
        private bool ToutEstValide()
        {
            bool ok = true;

            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible)
                ok = false;

            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible)
                ok = false;

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible)
                ok = false;

            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible)
                ok = false;

            

            return ok;
        }

        // Annuler Réinitialise le formulaire
        private void Annuler(object sender, RoutedEventArgs e)
        {
            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            txtAllergies.Clear();
            txtPlats.Clear();

            txtPrenom.BorderBrush = Brushes.Gray;
            txtNom.BorderBrush = Brushes.Gray;
            txtEmail.BorderBrush = Brushes.Gray;
            txtTelephone.BorderBrush = Brushes.Gray;

            erreurPrenom.Visibility = Visibility.Collapsed;
            erreurNom.Visibility = Visibility.Collapsed;
            erreurEmail.Visibility = Visibility.Collapsed;
            erreurTelephone.Visibility = Visibility.Collapsed;
        }

        // Bouton Créer client
        private void CreerClient(object sender, RoutedEventArgs e)
        {
            if (!ToutEstValide())
            {
                MessageBox.Show("champs non valide",
                                "Champs invalides",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            string prenom = txtPrenom.Text.Trim();
            string nom = txtNom.Text.Trim();

            MessageBox.Show($"Client {prenom} {nom} créé",
                            "Succès",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            Annuler(sender, e);
        }
    }
}