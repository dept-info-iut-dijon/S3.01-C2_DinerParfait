using EpicurApp_API.Models;
using EpicurAPP_Partage.Models;
using EpicurAppIHM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace EpicurAppIHM.Views
{
    public partial class FicheClient : Window
    {
        private HttpClient _httpClient;
        private List<Allergene> allergenes;
        private int? _clientId;
        private bool _modeConsultation;

        public FicheClient()
        {
            InitializeComponent();
            _httpClient = ApiClient.Instance;
            _modeConsultation = false;
            ChargerAllergenes();
        }

        public FicheClient(int clientId, bool modeConsultation = true)
        {
            InitializeComponent();
            _httpClient = ApiClient.Instance;
            _clientId = clientId;
            _modeConsultation = modeConsultation;

            ChargerAllergenes();
            ChargerClient();

            if (_modeConsultation)
            {
                ConfigurerModeConsultation();
            }
        }

        private async void ChargerAllergenes()
        {
            try
            {
                allergenes = await _httpClient.GetFromJsonAsync<List<Allergene>>("Allergenes");
                cmbAllergenes.ItemsSource = allergenes;
                cmbAllergenes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de charger les allergènes : " + ex.Message,
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ChargerClient()
        {
            if (!_clientId.HasValue) return;

            try
            {
                Client client = await _httpClient.GetFromJsonAsync<Client>($"Client/{_clientId.Value}");

                if (client == null)
                {
                    MessageBox.Show("Client introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                txtPrenom.Text = client.Prenom;
                txtNom.Text = client.Nom;
                txtEmail.Text = client.Email;
                txtTelephone.Text = client.Telephone;
                txtPlatsNonApprecies.Text = client.PlatsNonApprecies;
                txtPreferences.Text = client.Preferences;

              

                if (client.Allergenes != null && client.Allergenes.Count > 0)
                {
                    cmbAllergenes.ItemsSource = client.Allergenes;
                    cmbAllergenes.SelectedIndex = 0; 
                }
                else
                {
                    cmbAllergenes.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du client : " + ex.Message,
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }



        private void ConfigurerModeConsultation()
        {
            this.Title = "Consultation Client";
            txtPrenom.IsReadOnly = true;
            txtNom.IsReadOnly = true;
            txtEmail.IsReadOnly = true;
            txtTelephone.IsReadOnly = true;
            txtPlatsNonApprecies.IsReadOnly = true;
            txtPreferences.IsReadOnly = true;
            cmbAllergenes.IsEnabled = false;

            txtPrenom.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
            txtNom.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
            txtEmail.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
            txtTelephone.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
            txtPlatsNonApprecies.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
            txtPreferences.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));

            btnCreer.Visibility = Visibility.Collapsed;
            btnAnnuler.Content = "Fermer";
            btnAnnuler.Width = 180;
        }

        private void ValiderPrenom(object sender, RoutedEventArgs e)
        {
            if (_modeConsultation) return;
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
            if (_modeConsultation) return;
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
            if (_modeConsultation) return;
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
            if (_modeConsultation) return;
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
            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible) return false;
            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible) return false;
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible) return false;
            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible) return false;
            return true;
        }

        private void Annuler(object sender, RoutedEventArgs e)
        {
            if (_modeConsultation) { this.Close(); return; }

            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            txtPlatsNonApprecies.Clear();
            txtPreferences.Clear();

            borderPrenom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderNom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderEmail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderTelephone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));

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
                                "Champs invalides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnCreer.IsEnabled = false;
            btnCreer.Content = "Création en cours...";

            try
            {
                Client client = new Client
                {
                    Nom = txtNom.Text.Trim(),
                    Prenom = txtPrenom.Text.Trim(),
                    Telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", ""),
                    Email = txtEmail.Text.Trim(),
                    PlatsNonApprecies = txtPlatsNonApprecies.Text.Trim(),
                    Preferences = txtPreferences.Text.Trim()
                };

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Client", client);

                if (response.IsSuccessStatusCode)
                {
                    Client clientCree = await response.Content.ReadFromJsonAsync<Client>();

                    int? allergeneIdSelectionne = (cmbAllergenes.SelectedItem as Allergene)?.Id;

                    if (allergeneIdSelectionne.HasValue && clientCree != null)
                    {
                        await _httpClient.PostAsJsonAsync($"Client/{clientCree.Id}/allergenes", new List<int> { allergeneIdSelectionne.Value });
                    }

                    MessageBox.Show($"Client {client.Prenom} {client.Nom} créé avec succès !",
                                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {errorContent}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée",
                                "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnCreer.IsEnabled = true;
                btnCreer.Content = "Créer le client";
            }
        }
    }
}
