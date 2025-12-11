using EpicurAPP_Partage.Models;
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
        /// Liste des allergènes disponibles
        /// </summary>
        private List<Allergene> allergenes;
        /// <summary>
        /// Dictionnaire pour tracker les allergènes sélectionnés (ID -> IsSelected)
        /// </summary>
        private Dictionary<int, bool> _allergenesSelectionnes = new Dictionary<int, bool>();
        /// <summary>
        /// L'ID du client en consultation/modification
        /// </summary>
        private int? _clientId;
        private bool _modeModification;
        /// <summary>
        /// IDs des allergènes du client chargé
        /// </summary>
        private List<int> _allergenesClient = new List<int>();

        /// <summary>
        /// FicheClient Constructor pour création
        /// </summary>
        public FicheClient()
        {
            InitializeComponent();
            _modeModification = false;
            ChargerAllergenes();
        }

        /// <summary>
        /// FicheClient Constructor pour consultation/modification
        /// </summary>
        /// <param name="clientId">id du client a modifier/consulter</param>
        /// <param name="modeConsultation">mod de co,sultation de la fiche</param>
        public FicheClient(int clientId, bool modeConsultation = true)
        {
            InitializeComponent();
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
            // Charger d'abord le client pour obtenir ses allergènes, puis charger la liste des allergènes
            ChargerClientPuisAllergenes();
        }

        /// <summary>
        /// Charge le client puis les allergènes (nécessaire pour avoir _allergenesClient avant de charger la liste)
        /// </summary>
        private async void ChargerClientPuisAllergenes()
        {
            await ChargerClientAsync();
            ChargerAllergenes();
        }

        /// <summary>
        /// Charge la liste des allergènes depuis l'API
        /// </summary>
        /// <exception cref="Exception">Erreur de chargement des allergènes</exception>
        private async void ChargerAllergenes()
        {
            try
            {
                allergenes = await App.AllergeneRepository.GetAllAsync();

                // Initialiser le dictionnaire de sélection
                _allergenesSelectionnes.Clear();
                foreach (Allergene allergene in allergenes)
                {
                    _allergenesSelectionnes[allergene.Id] = _allergenesClient.Contains(allergene.Id);
                }

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
        /// <exception cref="Exception">Erreur de chargement du client</exception>
        private async void ChargerClient()
        {
            await ChargerClientAsync();
            ChargerAllergenes();
        }

        /// <summary>
        /// Charge les données du client de manière asynchrone
        /// </summary>
        /// <exception cref="Exception">Erreur de chargement du client</exception>
        private async Task ChargerClientAsync()
        {
            if (!_clientId.HasValue) return;

            try
            {
                Client client = await App.ClientRepository.GetByIdAsync(_clientId.Value);

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
        /// Gère l'expansion de l'Expander
        /// </summary>
        private void ExpanderAllergenes_Expanded(object sender, RoutedEventArgs e)
        {
            // Les événements des CheckBox gèrent maintenant la sélection
        }

        /// <summary>
        /// Initialise l'état de la CheckBox lors du chargement
        /// </summary>
        private void CheckBoxAllergene_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is int id)
            {
                checkBox.IsChecked = _allergenesSelectionnes.ContainsKey(id) && _allergenesSelectionnes[id];
            }
        }

        /// <summary>
        /// Met à jour le dictionnaire quand une CheckBox est cochée
        /// </summary>
        private void CheckBoxAllergene_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is int id)
            {
                _allergenesSelectionnes[id] = true;
            }
        }

        /// <summary>
        /// Met à jour le dictionnaire quand une CheckBox est décochée
        /// </summary>
        private void CheckBoxAllergene_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is int id)
            {
                _allergenesSelectionnes[id] = false;
            }
        }

        /// <summary>
        /// Récupère les IDs des allergènes cochés
        /// </summary>
        /// <returns>Liste des IDs cochés</returns>
        private List<int> GetAllergenesCoches()
        {
            return _allergenesSelectionnes
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>
        /// Configure la fenêtre en mode consultation
        /// </summary>
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
            SolidColorBrush texteBlanc = new SolidColorBrush(Colors.White);

            // Changer le background des Border au lieu des TextBox
            borderPrenom.Background = couleurConsultation;
            borderNom.Background = couleurConsultation;
            borderEmail.Background = couleurConsultation;
            borderTelephone.Background = couleurConsultation;
            borderPlatsNonApprecies.Background = couleurConsultation;
            borderPreferences.Background = couleurConsultation;
            borderAllergenes.Background = couleurConsultation;

            // Changer la couleur du texte en blanc
            txtPrenom.Foreground = texteBlanc;
            txtNom.Foreground = texteBlanc;
            txtEmail.Foreground = texteBlanc;
            txtTelephone.Foreground = texteBlanc;
            txtPlatsNonApprecies.Foreground = texteBlanc;
            txtPreferences.Foreground = texteBlanc;

            // Changer les couleurs de l'expander et de la liste des allergènes
            expanderAllergenes.Background = couleurConsultation;
            expanderAllergenes.Foreground = texteBlanc;
            lstAllergenes.Background = couleurConsultation;

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

            // Garder le même style qu'en consultation
            SolidColorBrush normalBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a2a2a"));
            SolidColorBrush normalForeground = new SolidColorBrush(Colors.White);

            // Restaurer le background des Border
            borderPrenom.Background = normalBackground;
            borderNom.Background = normalBackground;
            borderEmail.Background = normalBackground;
            borderTelephone.Background = normalBackground;
            borderPlatsNonApprecies.Background = normalBackground;
            borderPreferences.Background = normalBackground;
            borderAllergenes.Background = normalBackground;

            // Restaurer la couleur du texte
            txtPrenom.Foreground = normalForeground;
            txtNom.Foreground = normalForeground;
            txtEmail.Foreground = normalForeground;
            txtTelephone.Foreground = normalForeground;
            txtPlatsNonApprecies.Foreground = normalForeground;
            txtPreferences.Foreground = normalForeground;

            // Restaurer les couleurs de l'expander et de la liste des allergènes
            expanderAllergenes.Background = normalBackground;
            expanderAllergenes.Foreground = normalForeground;
            lstAllergenes.Background = normalBackground;

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

        /// <summary>
        /// Validation du prénom
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
        /// <exception cref="Exception">Erreur lors de la création/modification du client</exception>
        /// <exception cref="HttpRequestException">Erreur de connexion à l'API</exception>
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

                    bool success = await App.ClientRepository.UpdateAsync(clientModifie);

                    if (success)
                    {
                        List<int> allergenesCoches = GetAllergenesCoches();
                        await App.ClientRepository.UpdateAllergenesAsync(_clientId.Value, allergenesCoches);

                        MessageBox.Show("Modifications enregistrées",
                                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du client.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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

                    Client clientCree = await App.ClientRepository.CreateAsync(client);

                    List<int> allergenesCoches = GetAllergenesCoches();
                    if (clientCree != null && allergenesCoches.Count > 0)
                    {
                        await App.ClientRepository.UpdateAllergenesAsync(clientCree.Id, allergenesCoches);
                    }

                    MessageBox.Show($"Client {client.Prenom} {client.Nom} créé avec succès !",
                                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
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
        /// <exception cref="Exception">Erreur lors de la suppression du client</exception>
        private async void SupprimerClient(object sender, RoutedEventArgs e)
        {
            if (!_clientId.HasValue) return;

            MessageBoxResult result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer ce client ({txtPrenom.Text} {txtNom.Text}) ?",
                                                      "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No) return;

            try
            {
                bool success = await App.ClientRepository.DeleteAsync(_clientId.Value);

                if (success)
                {
                    MessageBox.Show("Client supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression du client.", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
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
