using EpicurAPP_Partage.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
        /// <summary>
        /// IDs des allergènes du client chargé
        /// </summary>
        private List<int> _allergenesClient = new List<int>();

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

            this.Title = "Consultation Fiche Client";

            if (modeConsultation)
            {
                ConfigurerModeConsultation();
            }
            else
            {
                // Mode modification directe
                btnSupprimer.Visibility = Visibility.Visible;
                btnModifier.Visibility = Visibility.Collapsed;
                btnCreer.Content = "Enregistrer";
                btnCreer.Visibility = Visibility.Visible;
            }
            ChargerAllergenes();
            ChargerClient();
        }

        /// <summary>
        /// Charge la liste des allergènes depuis l'API
        /// </summary>
        private async void ChargerAllergenes()
        {
            try
            {
                List<Allergene> listeAllergenes = await _httpClient.GetFromJsonAsync<List<Allergene>>("Allergenes");
                allergenes = listeAllergenes;
                lstAllergenes.ItemsSource = allergenes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de charger les allergènes : " + ex.Message,
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Charge les données du client
        /// </summary>
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

                // Champs texte avec protection null
                txtPrenom.Text = client.Prenom ?? string.Empty;
                txtNom.Text = client.Nom ?? string.Empty;
                txtEmail.Text = client.Email ?? string.Empty;
                txtTelephone.Text = client.Telephone ?? string.Empty;
                txtPreferences.Text = client.Preferences ?? string.Empty;

                // Plats non appréciés avec protection null
                if (client.PlatsNonApprecies != null && client.PlatsNonApprecies.Count > 0)
                {
                    IEnumerable<string> nomsPlats = client.PlatsNonApprecies
                        .Where(p => p != null && !string.IsNullOrEmpty(p.Nom))
                        .Select(p => p.Nom);
                    txtPlatsNonApprecies.Text = string.Join(", ", nomsPlats);
                }
                else
                {
                    txtPlatsNonApprecies.Text = string.Empty;
                }

                // Stocker les IDs des allergènes du client
                if (client.Allergenes != null && client.Allergenes.Count > 0)
                {
                    List<int> idsClient = client.Allergenes.Select(a => a.Id).ToList();
                    _allergenesClient = idsClient;

                    // Mettre à jour le header de l'Expander
                    int nbAllergenes = _allergenesClient.Count;
                    expanderAllergenes.Header = nbAllergenes > 0
                        ? $"Allergènes du client ({nbAllergenes})"
                        : "Aucun allergène";
                }
                else
                {
                    _allergenesClient.Clear();
                    expanderAllergenes.Header = "Aucun allergène";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement du client : " + ex.Message + "\n\nStack: " + ex.StackTrace,
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Trouve la CheckBox dans un élément de liste
        /// </summary>
        private CheckBox TrouverCheckBox(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is CheckBox checkBox)
                    return checkBox;

                CheckBox result = TrouverCheckBox(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Gère l'expansion de l'Expander pour cocher les allergènes du client
        /// </summary>
        private void ExpanderAllergenes_Expanded(object sender, RoutedEventArgs e)
        {
            // Forcer la génération des conteneurs
            lstAllergenes.UpdateLayout();

            // Cocher les allergènes correspondants
            foreach (object item in lstAllergenes.Items)
            {
                ContentPresenter container = lstAllergenes.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                if (container != null)
                {
                    CheckBox checkBox = TrouverCheckBox(container);
                    if (checkBox != null && checkBox.Tag != null)
                    {
                        int allergeneId = (int)checkBox.Tag;
                        checkBox.IsChecked = _allergenesClient.Contains(allergeneId);
                    }
                }
            }
        }

        /// <summary>
        /// Récupère les IDs des allergènes cochés
        /// </summary>
        private List<int> GetAllergenesCoches()
        {
            List<int> ids = new List<int>();

            foreach (object item in lstAllergenes.Items)
            {
                ContentPresenter container = lstAllergenes.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                if (container != null)
                {
                    CheckBox checkBox = TrouverCheckBox(container);
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        ids.Add((int)checkBox.Tag);
                    }
                }
            }

            return ids;
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
            lstAllergenes.IsEnabled = false;

            SolidColorBrush couleurConsultation = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3a3a3a"));
            txtPrenom.Background = couleurConsultation;
            txtNom.Background = couleurConsultation;
            txtEmail.Background = couleurConsultation;
            txtTelephone.Background = couleurConsultation;
            txtPlatsNonApprecies.Background = couleurConsultation;
            txtPreferences.Background = couleurConsultation;

            // Mettre à jour le header de l'Expander selon le nombre d'allergènes
            int nbAllergenes = _allergenesClient.Count;
            expanderAllergenes.Header = nbAllergenes > 0
                ? $"Allergènes du client ({nbAllergenes})"
                : "Aucun allergène";

            // Configuration des boutons pour la consultation avec modification, suppression ET historique
            btnCreer.Visibility = Visibility.Collapsed;
            btnHistorique.Visibility = Visibility.Visible;
            btnModifier.Visibility = Visibility.Visible;
            btnSupprimer.Visibility = Visibility.Visible;
            btnAnnuler.Content = "Fermer";
        }

        /// <summary>
        /// Active le mode modification à partir du mode consultation 
        /// </summary>
        private void ModifierClient(object sender, RoutedEventArgs e)
        {
            // Passer en mode modification
            this.Title = "Modification Fiche Client";

            txtPrenom.IsReadOnly = false;
            txtNom.IsReadOnly = false;
            txtEmail.IsReadOnly = false;
            txtTelephone.IsReadOnly = false;
            txtPlatsNonApprecies.IsReadOnly = false;
            txtPreferences.IsReadOnly = false;
            lstAllergenes.IsEnabled = true;

            // Restaurer l'apparence normale des champs
            SolidColorBrush normalBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a2a2a"));
            txtPrenom.Background = normalBackground;
            txtNom.Background = normalBackground;
            txtEmail.Background = normalBackground;
            txtTelephone.Background = normalBackground;
            txtPlatsNonApprecies.Background = normalBackground;
            txtPreferences.Background = normalBackground;

            // Modifier les boutons
            btnHistorique.Visibility = Visibility.Collapsed;
            btnModifier.Visibility = Visibility.Collapsed;
            btnSupprimer.Visibility = Visibility.Collapsed;
            btnCreer.Visibility = Visibility.Visible;
            btnCreer.Content = "Enregistrer";
            btnAnnuler.Content = "Annuler";

            // Changer le header de l'Expander en mode modification
            expanderAllergenes.Header = "Sélectionner les allergènes";
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
        private bool ToutEstValide()
        {
            bool res = true;
            if (string.IsNullOrWhiteSpace(txtPrenom.Text) || erreurPrenom.Visibility == Visibility.Visible) res = false;
            if (string.IsNullOrWhiteSpace(txtNom.Text) || erreurNom.Visibility == Visibility.Visible) res = false;
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || erreurEmail.Visibility == Visibility.Visible) res = false;
            if (string.IsNullOrWhiteSpace(txtTelephone.Text) || erreurTelephone.Visibility == Visibility.Visible) res = false;
            return res;
        }

        /// <summary>
        /// Efface les champs du formulaire ou ferme la fenêtre en mode consultation
        /// </summary>
        private void Annuler(object sender, RoutedEventArgs e)
        {
            // Si on est en mode modification, retourner au mode consultation
            if (btnCreer.Visibility == Visibility.Visible && _modeModification && _clientId.HasValue)
            {
                MessageBoxResult result = MessageBox.Show("Voulez-vous abandonner les modifications ?",
                                           "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Recharger les données originales et retourner en mode consultation
                    ChargerClient();
                    ConfigurerModeConsultation();
                }
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Crée ou modifie un client via l'API
        /// </summary>
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
                MessageBoxResult confirmation = MessageBox.Show("Voulez-vous enregistrer les modifications ?",
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
                        Preferences = txtPreferences.Text.Trim()
                    };

                    HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Client/{_clientId.Value}", clientModifie);

                    if (response.IsSuccessStatusCode)
                    {
                        List<int> allergenesCoches = GetAllergenesCoches();
                        await _httpClient.PostAsJsonAsync($"Client/{_clientId.Value}/allergenes", allergenesCoches);

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
                    btnCreer.Content = "Enregistrer";
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
                        Preferences = txtPreferences.Text.Trim()
                    };

                    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Client", client);

                    if (response.IsSuccessStatusCode)
                    {
                        Client clientCree = await response.Content.ReadFromJsonAsync<Client>();

                        List<int> allergenesCoches = GetAllergenesCoches();
                        if (clientCree != null && allergenesCoches.Count > 0)
                        {
                            await _httpClient.PostAsJsonAsync($"Client/{clientCree.Id}/allergenes", allergenesCoches);
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

        /// <summary>
        /// Supprime le client actuel 
        /// </summary>
        private async void SupprimerClient(object sender, RoutedEventArgs e)
        {
            if (!_clientId.HasValue) return;

            MessageBoxResult result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer ce client ({txtPrenom.Text} {txtNom.Text}) ?",
                                                      "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No) return;

            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Client/{_clientId}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Client supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur lors de la suppression : {error}", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur technique : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ouvre la fenêtre d'historique des repas pour ce client 
        /// </summary>
        private void VoirHistorique(object sender, RoutedEventArgs e)
        {
            if (!_clientId.HasValue) return;

            string nomComplet = $"{txtPrenom.Text} {txtNom.Text}";
            HistoriqueRepas fenetreHistorique = new HistoriqueRepas(_clientId.Value, nomComplet);
            fenetreHistorique.ShowDialog();
        }
    }
}
