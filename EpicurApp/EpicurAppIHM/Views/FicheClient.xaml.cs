using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using EpicurApp_API.Models;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.Services;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Fenêtre de création d'un nouveau client
    /// </summary>
    public partial class FicheClient : Window
    {
        private HttpClient _httpClient;

        public FicheClient()
        {
            InitializeComponent();

            _httpClient = ApiClient.Instance;

            ChargerAllergenes();
        }

        private async void ChargerAllergenes()
        {
            try
            {
                Allergene[] allergenes = await _httpClient.GetFromJsonAsync<Allergene[]>("Allergenes");
                cmbAllergenes.ItemsSource = allergenes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de charger les allergènes : " + ex.Message,
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        #region Validation champs

        private void ValiderPrenom(object sender, RoutedEventArgs e)
        {
            string prenom = txtPrenom.Text.Trim();

            if (!string.IsNullOrEmpty(prenom) && !Regex.IsMatch(prenom, @"^[a-zA-ZÀ-ÿ\s'-]+$"))
            {
                borderPrenom.BorderBrush = Brushes.Red;
                erreurPrenom.Visibility = Visibility.Visible;
            }
            else
            {
                borderPrenom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
                erreurPrenom.Visibility = Visibility.Collapsed;
            }
        }

        private void ValiderNom(object sender, RoutedEventArgs e)
        {
            string nom = txtNom.Text.Trim();

            if (!string.IsNullOrEmpty(nom) && !Regex.IsMatch(nom, @"^[a-zA-ZÀ-ÿ\s'-]+$"))
            {
                borderNom.BorderBrush = Brushes.Red;
                erreurNom.Visibility = Visibility.Visible;
            }
            else
            {
                borderNom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
                erreurNom.Visibility = Visibility.Collapsed;
            }
        }

        private void ValiderEmail(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                borderEmail.BorderBrush = Brushes.Red;
                erreurEmail.Visibility = Visibility.Visible;
            }
            else
            {
                borderEmail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
                erreurEmail.Visibility = Visibility.Collapsed;
            }
        }

        private void ValiderTelephone(object sender, RoutedEventArgs e)
        {
            string telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", "");

            if (!string.IsNullOrEmpty(telephone) && !Regex.IsMatch(telephone, @"^0[1-9]\d{8}$"))
            {
                borderTelephone.BorderBrush = Brushes.Red;
                erreurTelephone.Visibility = Visibility.Visible;
            }
            else
            {
                borderTelephone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
                erreurTelephone.Visibility = Visibility.Collapsed;
            }
        }

        private bool ToutEstValide()
        {
            bool valide = true;

            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible)
            {
                valide = false;
            }

            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible)
            {
                valide = false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible)
            {
                valide = false;
            }

            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible)
            {
                valide = false;
            }

            return valide;
        }

        #endregion

        private void Annuler(object sender, RoutedEventArgs e)
        {
            // Vider les champs
            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            cmbAllergenes.SelectedIndex = -1;
            txtPlats.Clear();

            // Réinitialiser les bordures
            borderPrenom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderNom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderEmail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderTelephone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));

            // Masquer les messages d'erreur
            erreurPrenom.Visibility = Visibility.Collapsed;
            erreurNom.Visibility = Visibility.Collapsed;
            erreurEmail.Visibility = Visibility.Collapsed;
            erreurTelephone.Visibility = Visibility.Collapsed;
        }

        private async void CreerClient(object sender, RoutedEventArgs e)
        {
            if (!ToutEstValide())
            {
                MessageBox.Show("Veuillez corriger les erreurs dans le formulaire",
                                "Champs invalides",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            btnCreer.IsEnabled = false;
            btnCreer.Content = "Création en cours...";

            try
            {
                string selectedAllergene = "";
                if (cmbAllergenes.SelectedItem is Allergene allergene)
                {
                    selectedAllergene = allergene.Nom;
                }

                Client client = new Client();
                client.Id = 0;
                client.Nom = txtNom.Text.Trim();
                client.Prenom = txtPrenom.Text.Trim();
                client.Telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", "");
                client.Email = txtEmail.Text.Trim();
                client.Allergies = selectedAllergene;
                client.Notes = txtPlats.Text.Trim();

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Client", client);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Client " + client.Prenom + " " + client.Nom + " créé avec succès !",
                                    "Succès",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    Annuler(sender, e);
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Erreur lors de la création :\n" + errorContent,
                                    "Erreur",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée (ex: http://localhost:8080)",
                                "Erreur de connexion",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur inattendue :\n" + ex.Message,
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                btnCreer.IsEnabled = true;
                btnCreer.Content = "Créer le client";
            }
        }
    }
}
