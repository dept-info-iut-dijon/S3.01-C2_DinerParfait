using EpicurAPP_Partage.Models;
using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Logique d'interaction pour FicheIdee.xaml
    /// </summary>
    public partial class FicheIdee : Window
    {
        /// <summary>
        /// ID de l'idée en cours de modification (null si création)
        /// </summary>
        private int? _ideeId;

        /// <summary>
        /// Constructeur pour créer une nouvelle idée
        /// </summary>
        public FicheIdee()
        {
            InitializeComponent();
            _ideeId = null;
        }

        /// <summary>
        /// Constructeur pour modifier une idée existante
        /// </summary>
        public FicheIdee(int ideeId)
        {
            InitializeComponent();
            _ideeId = ideeId;

            this.Title = "Modifier Idée";
            btnSupprimer.Visibility = Visibility.Visible;
            ChargerIdee();
        }

        /// <summary>
        /// Charge les données de l'idée depuis l'API
        /// </summary>
        /// <exception cref="Exception">Erreur de chargement de l'idée</exception>
        private async void ChargerIdee()
        {
            if (!_ideeId.HasValue) return;

            try
            {
                IdeePlat idee = await App.IdeePlatRepository.GetByIdAsync(_ideeId.Value);

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
        /// <exception cref="Exception">Erreur lors de l'enregistrement</exception>
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

                bool success;

                if (_ideeId.HasValue)
                {
                    // Mode modification
                    idee.Id = _ideeId.Value;
                    success = await App.IdeePlatRepository.UpdateAsync(idee);
                }
                else
                {
                    // Mode création
                    IdeePlat ideeCreee = await App.IdeePlatRepository.CreateAsync(idee);
                    success = ideeCreee != null;
                }

                if (success)
                {
                    MessageBox.Show("Idée enregistrée avec succès !",
                                   "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement de l'idée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// <exception cref="Exception">Erreur lors de la suppression</exception>
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
                bool success = await App.IdeePlatRepository.DeleteAsync(_ideeId.Value);

                if (success)
                {
                    MessageBox.Show("Idée supprimée avec succès", "Succès",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression de l'idée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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
