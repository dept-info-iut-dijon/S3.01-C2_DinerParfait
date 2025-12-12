using EpicurAPP_Partage.Models;
using System.Windows;

namespace EpicurAppIHM.Views
{
    public partial class ConsultationMenu : Window
    {
        private int _menuId;
        private string _menuStatut = "";

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
                // S'assurer que le header X-Restaurant-Id est défini
                if (App.CurrentRestaurant != null)
                {
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);
                }

                // Récupérer le menu depuis l'API
                Menu? menu = await App.MenuRepository.GetByIdAsync(_menuId);

                if (menu == null)
                {
                    MessageBox.Show("Menu introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // Récupérer tous les plats pour afficher les noms
                List<Plat> plats = await App.PlatRepository.GetAllAsync();

                // Afficher les informations générales
                txtNom.Text = menu.Nom;
                txtStatut.Text = menu.Statut;
                _menuStatut = menu.Statut;

                // Afficher le bouton modifier pour les brouillons
                if (menu.Statut == "Brouillon")
                {
                    btnModifier.Visibility = Visibility.Visible;

                    // Désactiver si le menu est utilisé dans un service
                    if (menu.EstUtilise)
                    {
                        btnModifier.IsEnabled = false;
                        btnModifier.ToolTip = "Ce menu est utilisé dans un service et ne peut pas être modifié.";
                        btnModifier.Opacity = 0.5;
                    }
                    else
                    {
                        btnModifier.IsEnabled = true;
                        btnModifier.ToolTip = "Modifier ce menu brouillon.";
                        btnModifier.Opacity = 1.0;
                    }
                }

                // Récupérer le premier plat de chaque catégorie et afficher son nom
                ElementMenu? amuseBouche = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.AmuseBouche);
                txtAmuseBouche.Text = ObtenirNomPlat(plats, amuseBouche?.PlatId);

                ElementMenu? boissonAperitif = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.BoissonAperitif);
                txtBoissonAperitif.Text = ObtenirNomPlat(plats, boissonAperitif?.PlatId);

                ElementMenu? entree = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Entree);
                txtEntree.Text = ObtenirNomPlat(plats, entree?.PlatId);

                ElementMenu? platPrincipal = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.PlatPrincipal);
                txtPlatPrincipal.Text = ObtenirNomPlat(plats, platPrincipal?.PlatId);

                ElementMenu? vin = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Vin);
                txtVin.Text = ObtenirNomPlat(plats, vin?.PlatId);

                ElementMenu? fromage = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Fromage);
                txtFromage.Text = ObtenirNomPlat(plats, fromage?.PlatId);

                ElementMenu? dessert = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Dessert);
                txtDessert.Text = ObtenirNomPlat(plats, dessert?.PlatId);
            }
            catch (Exception ex)
            {
                string detailErreur = $"Erreur : {ex.Message}";
                if (ex.InnerException != null)
                {
                    detailErreur += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                detailErreur += $"\n\nStack Trace: {ex.StackTrace}";

                MessageBox.Show($"Erreur lors du chargement du menu :\n\n{detailErreur}",
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

            Plat? plat = plats.Find(p => p.Id == platId.Value);
            return plat?.Nom ?? "-";
        }

        /// <summary>
        /// Génère et affiche la liste de courses pour le menu actuel
        /// </summary>
        /// <exception cref="Exception">Erreur de génération de la liste de courses</exception>
        private async void GenererListeCourses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Désactiver le bouton pendant la génération
                btnGenererListeCourses.IsEnabled = false;
                btnGenererListeCourses.Content = "Génération en cours...";

                // Appeler le repository pour générer la liste de courses
                var listeCourses = await App.MenuRepository.GetListeCoursesAsync(_menuId);

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
        /// <exception cref="Exception">Erreur de suppression du menu</exception>
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

                    bool success = await App.MenuRepository.DeleteAsync(_menuId);

                    if (success)
                    {
                        MessageBox.Show("Menu supprimé avec succès.", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression du menu.",
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
        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Ouvre la fenêtre d'édition pour modifier le brouillon
        /// </summary>
        private void Modifier_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de création en mode édition
            CreationMenu creationMenu = new CreationMenu(_menuId);
            creationMenu.ShowDialog();

            // Fermer la fenêtre de consultation
            this.Close();
        }
    }
}
