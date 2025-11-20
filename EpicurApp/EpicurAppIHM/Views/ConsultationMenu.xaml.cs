using EpicurAPP_Partage.Models;
using System.Net.Http.Json;
using System.Windows;

namespace EpicurAppIHM.Views
{
    public partial class ConsultationMenu : Window
    {
        private int _menuId;

        /// <summary>
        /// Intancie la fenetre avec les menus
        /// </summary>
        /// <param name="menuId">Le menu cible</param>
        public ConsultationMenu(int menuId)
        {
            InitializeComponent();
            _menuId = menuId;
            ChargerMenu();
        }

        /// <summary>
        /// Charge les menus
        /// </summary>
        /// <exception cref="Exception">Erreur de chargement du menus</exception>
        private async void ChargerMenu()
        {
            try
            {
                // Récupérer le menu depuis l'API
                Menu menu = await App.ApiClient.HttpClient.GetFromJsonAsync<Menu>($"Menu/{_menuId}");

                if (menu == null)
                {
                    MessageBox.Show("Menu introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // Récupérer tous les plats pour afficher les noms
                List<Plat> plats = await App.ApiClient.HttpClient.GetFromJsonAsync<List<Plat>>("Plats");

                // Afficher les informations générales
                txtNom.Text = menu.Nom;
                txtDate.Text = menu.Date.ToString("dd/MM/yyyy");
                txtStatut.Text = menu.Statut;

                // Afficher les plats (avec vérification pour les IDs null)
                txtAmuseBouche.Text = ObtenirNomPlat(plats, menu.AmuseBouche?.Id);
                txtBoissonAperitif.Text = ObtenirNomPlat(plats, menu.BoissonAperitif?.Id);
                txtEntree.Text = ObtenirNomPlat(plats, menu.Entree?.Id);
                txtPlatPrincipal.Text = ObtenirNomPlat(plats, menu.PlatPrincipal?.Id);
                txtVin.Text = ObtenirNomPlat(plats, menu.Vin?.Id);
                txtFromage.Text = ObtenirNomPlat(plats, menu.Fromage?.Id);
                txtDessert.Text = ObtenirNomPlat(plats, menu.Dessert?.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du menu : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Obtient le nom d'un plat depuis la liste
        /// </summary>
        /// <param name="plats">plats disponibles</param>
        /// <param name="platId">plat cible</param>
        /// <returns>Nom du plat cible</returns>
        private string ObtenirNomPlat(List<Plat> plats, int? platId)
        {
            if (platId == null || plats == null)
                return "-";

            Plat plat = plats.Find(p => p.Id == platId.Value);
            return plat?.Nom ?? "-";
        }

        /// <summary>
        /// Génère et affiche la liste de courses pour le menu actuel
        /// </summary>
        private async void GenererListeCourses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Désactiver le bouton pendant la génération
                btnGenererListeCourses.IsEnabled = false;
                btnGenererListeCourses.Content = "Génération en cours...";

                // Appeler l'API pour générer la liste de courses
                var response = await App.ApiClient.HttpClient.GetAsync($"Menu/{_menuId}/courses");
                
                if (response.IsSuccessStatusCode)
                {
                    var listeCourses = await response.Content.ReadFromJsonAsync<List<ElementListeCourse>>();
                    
                    if (listeCourses != null && listeCourses.Any())
                    {
                        // Ouvrir la fenêtre d'affichage de la liste de courses
                        var fenetreListeCourses = new AffichageListeCourses(listeCourses, txtNom.Text);
                        fenetreListeCourses.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Aucun ingrédient trouvé pour ce menu.", "Information", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Erreur lors de la génération de la liste de courses.", "Erreur", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération de la liste de courses : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnGenererListeCourses.IsEnabled = true;
                btnGenererListeCourses.Content = "Générer la liste de courses";
            }
        }

        /// <summary>
        /// Supprime le menu actuel
        /// </summary>
        private async void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmation = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer ce menu ?\nCette action est irréversible.",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation == MessageBoxResult.Yes)
            {
                try
                {
                    btnSupprimer.IsEnabled = false;
                    btnSupprimer.Content = "Suppression...";

                    var response = await App.ApiClient.HttpClient.DeleteAsync($"Menu/{_menuId}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Menu supprimé avec succès.", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        string errorDetails = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erreur lors de la suppression du menu :\n{errorDetails}",
                            "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        btnSupprimer.IsEnabled = true;
                        btnSupprimer.Content = "Supprimer";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression du menu : {ex.Message}",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnSupprimer.IsEnabled = true;
                    btnSupprimer.Content = "Supprimer";
                }
            }
        }

        /// <summary>
        /// Ferme la fenetre
        /// </summary>
        /// <param name="sender">Envoie</param>
        /// <param name="e">Argument</param>
        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
