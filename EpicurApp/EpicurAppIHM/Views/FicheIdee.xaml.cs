using EpicurAPP_Partage.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Logique d'interaction pour FicheIdee.xaml
    /// </summary>
    public partial class FicheIdee : Window
    {
        private readonly HttpClient _httpClient;
        private int? _ideeId;

        /// <summary>
        /// Constructeur pour créer une nouvelle idée
        /// </summary>
        public FicheIdee()
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            _ideeId = null;
        }

        /// <summary>
        /// Constructeur pour modifier une idée existante
        /// </summary>
        public FicheIdee(int ideeId)
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            _ideeId = ideeId;

            this.Title = "Modifier Idée";
            btnSupprimer.Visibility = Visibility.Visible;
            ChargerIdee();
        }

        /// <summary>
        /// Charge les données de l'idée depuis l'API
        /// </summary>
        private async void ChargerIdee()
        {
            if (!_ideeId.HasValue) return;

            try
            {
                List<IdeePlat> idees = await _httpClient.GetFromJsonAsync<List<IdeePlat>>("IdeePlat");
                IdeePlat idee = idees != null ? idees.FirstOrDefault(i => i.Id == _ideeId.Value) : null;

                if (idee == null)
                {
                    MessageBox.Show("Idée introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                txtTitre.Text = idee.Titre ?? "";
                txtDescription.Text = idee.Description ?? "";
                txtNotes.Text = idee.Notes ?? "";

                // Sélectionner la catégorie
                foreach (ComboBoxItem item in cmbCategorie.Items)
                {
                    if (item.Content.ToString() == idee.Categorie)
                    {
                        cmbCategorie.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Enregistre l'idée (création ou modification)
        /// </summary>
        private async void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtTitre.Text))
            {
                MessageBox.Show("Le titre est obligatoire", "Champ manquant",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IdeePlat idee = new IdeePlat
                {
                    Titre = txtTitre.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Notes = txtNotes.Text.Trim(),
                    Categorie = (cmbCategorie.SelectedItem as ComboBoxItem)?.Content.ToString() ?? ""
                };

                HttpResponseMessage response;

                if (_ideeId.HasValue)
                {
                    // Mode modification
                    idee.Id = _ideeId.Value;
                    response = await _httpClient.PutAsJsonAsync($"IdeePlat/{_ideeId.Value}", idee);
                }
                else
                {
                    // Mode création
                    response = await _httpClient.PostAsJsonAsync("IdeePlat", idee);
                }

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Idée enregistrée avec succès !",
                                   "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {error}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Supprime l'idée
        /// </summary>
        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (!_ideeId.HasValue) return;

            MessageBoxResult result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer cette idée ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.No) return;

            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"IdeePlat/{_ideeId.Value}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Idée supprimée avec succès", "Succès",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {error}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ferme la fenêtre
        /// </summary>
        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
