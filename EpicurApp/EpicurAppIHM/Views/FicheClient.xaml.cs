using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Fenêtre de création d'un nouveau client
    /// </summary>
    public partial class FicheClient : Window
    {
        private readonly HttpClient _httpClient;

        public FicheClient()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7068/")
            };
        }

        /// <summary>
        /// Valide le prénom en temps réel
        /// </summary>
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

        /// <summary>
        /// Valide le nom en temps réel
        /// </summary>
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

        /// <summary>
        /// Valide l'email en temps réel
        /// </summary>
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

        /// <summary>
        /// Valide le téléphone en temps réel
        /// </summary>
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

        /// <summary>
        /// Vérifie que tous les champs sont valides
        /// </summary>
        private bool ToutEstValide()
        {
            bool valide = true;

            // Vérifier prénom
            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible)
                valide = false;

            // Vérifier nom
            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible)
                valide = false;

            // Vérifier email
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible)
                valide = false;

            // Vérifier téléphone
            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible)
                valide = false;

            return valide;
        }

        /// <summary>
        /// Réinitialise tous les champs du formulaire
        /// </summary>
        private void Annuler(object sender, RoutedEventArgs e)
        {
            // Vider les champs
            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            txtAllergies.Clear();
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

        /// <summary>
        /// Crée un nouveau client en appelant l'API
        /// </summary>
        private async void CreerClient(object sender, RoutedEventArgs e)
        {
            // Validation globale du formulaire
            if (!ToutEstValide())
            {
                MessageBox.Show("Veuillez corriger les erreurs dans le formulaire",
                                "Champs invalides",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Désactivation du bouton pendant la requête
            btnCreer.IsEnabled = false;
            btnCreer.Content = "Création en cours...";

            try
            {
                // Préparation des données à envoyer à l'API
                var clientData = new
                {
                    id = 0,
                    nom = txtNom.Text.Trim(),
                    prenom = txtPrenom.Text.Trim(),
                    telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", ""),
                    email = txtEmail.Text.Trim(),
                    allergies = txtAllergies.Text.Trim(),
                    notes = txtPlats.Text.Trim()
                };

                // Envoi de la requête POST à l'API
                var response = await _httpClient.PostAsJsonAsync("Client", clientData);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Client {clientData.prenom} {clientData.nom} créé avec succès !",
                                    "Succès",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    // Réinitialisation du formulaire
                    Annuler(sender, e);
                }
                else
                {
                    // Lecture du message d'erreur de l'API
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur lors de la création :\n{errorContent}",
                                    "Erreur",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée (https://localhost:7068)",
                                "Erreur de connexion",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur inattendue :\n{ex.Message}",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                // Réactivation du bouton
                btnCreer.IsEnabled = true;
                btnCreer.Content = "Créer le client";
            }
        }
    }
}