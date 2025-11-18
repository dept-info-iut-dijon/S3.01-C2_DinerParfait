using EpicurAPP_Partage.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Logique d'interaction pour FicheClient.xaml
    /// </summary>
    public partial class FicheClient : Window
    {
        /// <summary>
        /// L'instance HttpClient pour les appels API
        /// </summary>
        private HttpClient _httpClient;
        /// <summary>
        /// Liste des allergènes disponibles
        /// </summary>
        private List<Allergene> allergenes;
        /// <summary>
        /// L'ID du client en consultation/modification
        /// </summary>
        private int? _clientId;
        private bool _modeModification;

        public FicheClient()
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            _modeModification = false;
            ChargerAllergenes();
        }

        public FicheClient(int clientId, bool modeConsultation = true)
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            _clientId = clientId;
            _modeModification = true;

            this.Title = "Modification Fiche Client";
            btnCreer.Content = "Modifier";

            ChargerAllergenes();
            ChargerClient();
        }

        /// <summary>
        /// Charge la liste des allergènes depuis l'API
        /// </summary>
        /// <exception cref="Exception">erreur de chargement des allergènes</exception>
        private async void ChargerAllergenes()
        {
            try
            {
                allergenes = await _httpClient.GetFromJsonAsync<List<Allergene>>("Allergenes");
                allergenes.Insert(0, new Allergene { Id = -1, Nom = "Aucun", Description = "" });

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
                // Convertir la liste de plats en string pour l'affichage
                txtPlatsNonApprecies.Text = client.PlatsNonApprecies != null && client.PlatsNonApprecies.Count > 0
                    ? string.Join(", ", client.PlatsNonApprecies.Select(p => p.Nom))
                    : string.Empty;
                txtPreferences.Text = client.Preferences;

                if (client.Allergenes != null && client.Allergenes.Count > 0)
                {
                    var allergeneClient = client.Allergenes.First();
                    var allergeneItem = allergenes.FirstOrDefault(a => a.Id == allergeneClient.Id);
                    if (allergeneItem != null)
                    {
                        cmbAllergenes.SelectedItem = allergeneItem;
                    }
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
        /// Validation du nom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Validation de l'email
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
        /// Validation du téléphone
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
        /// Vérifie si tous les champs sont valides
        /// </summary>
        /// <returns>renvoie true si tout les champs obligatoires sont remplis</returns>
        private bool ToutEstValide()
        {
            bool res = true;
            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible) res = false;
            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible) res = false;
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible) res = false;
            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible) res =  false;
            return res;
        }

        /// <summary>
        /// Efface les champs du formulaire ou ferme la fenêtre en mode consultation
        /// </summary>
        private void Annuler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Crée un nouveau client via l'API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception">erreur lors de la création du client</exception>
        /// <exception cref="HttpRequestException">erreur de connexion à l'API</exception>
        private async void CreerClient(object sender, RoutedEventArgs e)
        {
            if (!ToutEstValide())
            {
                MessageBox.Show("Veuillez corriger les erreurs dans le formulaire",
                                "Champs invalides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // MODE MODIFICATION
            if (_modeModification)
            {
                var confirmation = MessageBox.Show("Voulez-vous enregistrer les modifications ?",
                                                 "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmation != MessageBoxResult.Yes)
                    return;

                btnCreer.IsEnabled = false;
                btnCreer.Content = "Modification en cours...";

                try
                {
                    Client clientModifie = new Client
                    {
                        Id = _clientId.Value,
                        Nom = txtNom.Text.Trim(),
                        Prenom = txtPrenom.Text.Trim(),
                        Telephone = txtTelephone.Text.Trim().Replace(" ", "").Replace("-", ""),
                        Email = txtEmail.Text.Trim(),
                        //PlatsNonApprecies = txtPlatsNonApprecies.Text.Trim(),
                        Preferences = txtPreferences.Text.Trim()
                    };

                    HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Client/{_clientId.Value}", clientModifie);

                    if (response.IsSuccessStatusCode)
                    {
                        int? allergeneIdSelectionne = (cmbAllergenes.SelectedItem as Allergene)?.Id;

                        if (allergeneIdSelectionne.HasValue && allergeneIdSelectionne.Value != -1)
                        {
                            await _httpClient.PostAsJsonAsync($"Client/{_clientId.Value}/allergenes", new List<int> { allergeneIdSelectionne.Value });
                        }

                        MessageBox.Show("Modifications enregistrées",
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
                    btnCreer.Content = "Modifier";
                }
            }
            // MODE CRÉATION
            else
            {
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
                    //PlatsNonApprecies = txtPlatsNonApprecies.Text.Trim(),
                    Preferences = txtPreferences.Text.Trim()
                };

                    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Client", client);

                    if (response.IsSuccessStatusCode)
                    {
                        Client clientCree = await response.Content.ReadFromJsonAsync<Client>();

                        int? allergeneIdSelectionne = (cmbAllergenes.SelectedItem as Allergene)?.Id;

                        if (allergeneIdSelectionne.HasValue && allergeneIdSelectionne.Value != -1 && clientCree != null)
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
}