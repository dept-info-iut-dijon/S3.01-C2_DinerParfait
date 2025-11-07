using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using EpicurAppIHM.Views;

namespace EpicurAppIHM
{
    /// <summary>
    /// Fenêtre de création d'un client
    /// </summary>
    public partial class FicheClient : Window
    {
        // Pour faire les appels à l'API
        private readonly HttpClient _httpClient;

        //créer un nouveau client
        public FicheClient()
        {
            InitializeComponent();

            // Configuration de l'adresse de l'API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7068/")
            };
        }

        // Constructeur pour afficher les infos d'un client existant
        public FicheClient(ClientDto client) : this()
        {
            // On remplit les champs avec les données du client
            txtPrenom.Text = client.Prenom;
            txtNom.Text = client.Nom;
            txtEmail.Text = client.Email;
            txtTelephone.Text = client.Telephone;
            txtAllergies.Text = client.Allergies ?? "";

            
            if (!string.IsNullOrWhiteSpace(client.Notes))
            {
                if (client.Notes.Contains("|"))
                {
                    var parts = client.Notes.Split('|');
                    if (parts.Length >= 1)
                        txtPlatsNonApprecies.Text = parts[0].Trim();
                    if (parts.Length >= 2)
                        txtPreferences.Text = parts[1].Trim();
                }
                else
                {
                    txtPreferences.Text = client.Notes;
                }
            }
        }

        // Vérifie que le prénom contient que des lettres
        private void ValiderPrenom(object sender, RoutedEventArgs e)
        {
            string prenom = txtPrenom.Text.Trim();

            // Si le prénom contient des caractères interdits
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

        // Vérifie que le nom contient que des lettres
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

        // Vérifie que l'email est au bon format
        private void ValiderEmail(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();

            // Vérification du format email ( @ et un point)
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

        // Vérifie que le téléphone a 10 chiffres et commence par 0
        private void ValiderTelephone(object sender, RoutedEventArgs e)
        {
            // espaces tirets
            string telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", "");

            // Le numéro doit commencer par 0 et avoir 10 chiffres
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

        // verifie que tous les champs obligatoires sont remplis
        private bool ToutEstValide()
        {
            bool valide = true;

          
            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible)
                valide = false;

            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible)
                valide = false;

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible)
                valide = false;

            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible)
                valide = false;

            return valide;
        }

        // Bouton Annuler 
        private void Annuler(object sender, RoutedEventArgs e)
        {
            // Vider tous les champs
            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            txtAllergies.Clear();
            txtPlatsNonApprecies.Clear();
            txtPreferences.Clear();

            // Remettre les bordures en doré
            borderPrenom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderNom.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderEmail.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));
            borderTelephone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D4AF37"));

            // Cacher les messages d'erreur
            erreurPrenom.Visibility = Visibility.Collapsed;
            erreurNom.Visibility = Visibility.Collapsed;
            erreurEmail.Visibility = Visibility.Collapsed;
            erreurTelephone.Visibility = Visibility.Collapsed;
        }

        // Bouton Créer : envoie les données à l'API pour créer le client
        private async void CreerClient(object sender, RoutedEventArgs e)
        {
            // verifier que tout est bon avant d envoyer
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
                
                var clientData = new
                {
                    id = 0,
                    nom = txtNom.Text.Trim(),
                    prenom = txtPrenom.Text.Trim(),
                    telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", ""),
                    email = txtEmail.Text.Trim(),
                    allergies = txtAllergies.Text.Trim(),
                    notes = (txtPlatsNonApprecies.Text + " " + txtPreferences.Text).Trim()
                };

                var response = await _httpClient.PostAsJsonAsync("Client", clientData);

                // Si marche
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Client {clientData.prenom} {clientData.nom} créé avec succès !",
                                    "Succès",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    // Fermer la fenêtre
                    this.Close();
                }
                else
                {
                    // Si API renvoie une erreur
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {errorContent}",
                                    "Erreur",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException)
            {
                // arrive pas à contacter API
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée.",
                                "Erreur de connexion",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Autre erreur
                MessageBox.Show($"Erreur : {ex.Message}",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            finally
            {
                // Reactiver bouton
                btnCreer.IsEnabled = true;
                btnCreer.Content = "Créer le client";
            }
        }
    }
}