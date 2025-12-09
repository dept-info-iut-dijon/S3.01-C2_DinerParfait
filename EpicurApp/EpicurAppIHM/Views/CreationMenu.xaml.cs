using EpicurAPP_Partage.Models;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using MenuModel = EpicurAPP_Partage.Models.Menu;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Fiche de création menu
    /// </summary>
    public partial class CreationMenu : Window
    {
        /// <summary>
        /// Liste des plats
        /// </summary>
        private List<Plat> tousLesPlats;
        /// <summary>
        /// Id du brouillon du menu
        /// </summary>
        private int? _menuBrouillonId;

        /// <summary>
        /// Intancie la fiche de creation menu
        /// </summary>
        /// <param name="menuId">ID du menu à charger (optionnel, charge le dernier brouillon si null)</param>
        public CreationMenu(int? menuId = null)
        {
            InitializeComponent();

            ChargerPlats();

            if (menuId.HasValue)
            {
                ChargerMenu(menuId.Value);
            }
            else
            {
                ChargerBrouillon();
            }

            btnAnnuler.Click += Annuler;
            btnSupprimer.Click += SupprimerMenu;
            btnEnregistrerBrouillon.Click += EnregistrerBrouillon;
            btnValider.Click += ValiderMenu;
        }

        /// <summary>
        /// Charge les plats
        /// </summary>
        /// <exception cref="Exception">API introuvable par le client</exception>
        private async void ChargerPlats()
        {
            try
            {
                // S'assurer que le header X-Restaurant-Id est défini
                if (App.CurrentRestaurant != null)
                {
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);
                }

                tousLesPlats = await App.PlatRepository.GetAllAsync();

                if (tousLesPlats != null && tousLesPlats.Count > 0)
                {
                    RemplirComboBox();
                }
                else
                {
                    MessageBox.Show("Aucun plat disponible dans la base de données", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                string detailErreur = $"Erreur : {ex.Message}";
                if (ex.InnerException != null)
                {
                    detailErreur += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                detailErreur += $"\n\nStack Trace: {ex.StackTrace}";
                detailErreur += $"\n\nURL de l'API: {App.ApiClient.HttpClient.BaseAddress}";
                detailErreur += $"\n\nRestaurant ID: {App.CurrentRestaurant?.Id}";

                MessageBox.Show($"Impossible de contacter l'API.\n\n{detailErreur}", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Remplit la combobox avec toutes les catégories de plats
        /// </summary>
        private void RemplirComboBox()
        {
            ConfigurerComboBox(cmbAmuseGueule, "AmuseBouche");
            ConfigurerComboBox(cmbBoissonAperitif, "BoissonAperitif");
            ConfigurerComboBox(cmbEntree, "Entree");
            ConfigurerComboBox(cmbPlat, "PlatPrincipal");
            ConfigurerComboBox(cmbVin, "Vin");
            ConfigurerComboBox(cmbFromage, "Fromage");
            ConfigurerComboBox(cmbDessert, "Dessert");
        }

        /// <summary>
        /// Configure une ComboBox avec les plats d'une catégorie donnée
        /// </summary>
        /// <param name="comboBox">combobox cible</param>
        /// <param name="categorieStr">catégorie du plat</param>
        private void ConfigurerComboBox(ComboBox comboBox, string categorieStr)
        {
            List<Plat> platsClasse = new List<Plat>();

            if (!Enum.TryParse<CategoriePlat>(categorieStr, out CategoriePlat categorieEnum))
                return;

            foreach (Plat plat in tousLesPlats)
            {
                if (plat.Categorie == categorieEnum)
                    platsClasse.Add(plat);
            }

            platsClasse.Sort(ComparerPlatsParNom);

            comboBox.ItemsSource = platsClasse;
            comboBox.DisplayMemberPath = "Nom";
            comboBox.SelectedValuePath = "Id";
            comboBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Compare deux plats par leur nom
        /// </summary>
        private int ComparerPlatsParNom(Plat p1, Plat p2)
        {
            if (p1 == null && p2 == null) return 0;
            if (p1 == null) return -1;
            if (p2 == null) return 1;
            if (p1.Nom == null && p2.Nom == null) return 0;
            if (p1.Nom == null) return -1;
            if (p2.Nom == null) return 1;
            return p1.Nom.CompareTo(p2.Nom);
        }

        /// <summary>
        /// Réinitialise la sélection des plats
        /// </summary>
        private void Annuler(object sender, RoutedEventArgs e)
        {
            ReinitialiserSelection();
        }

        /// <summary>
        /// vérifie qu'au moins un plat est sélectionné
        /// </summary>
        private bool ValidationMenu()
        {
            if (cmbAmuseGueule.SelectedItem == null &&
                cmbBoissonAperitif.SelectedItem == null &&
                cmbEntree.SelectedItem == null &&
                cmbPlat.SelectedItem == null &&
                cmbVin.SelectedItem == null &&
                cmbFromage.SelectedItem == null &&
                cmbDessert.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner au moins un plat", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enregistre le brouillon du menu
        /// </summary>
        private void EnregistrerBrouillon(object sender, RoutedEventArgs e)
        {
            EnregistrerMenu("Brouillon", false);
        }

        /// <summary>
        /// Valide le menu
        /// </summary>
        private void ValiderMenu(object sender, RoutedEventArgs e)
        {
            if (!ValidationMenu())
                return;

            EnregistrerMenu("Validé", true);
        }

        /// <summary>
        /// Charge un menu spécifique par son ID
        /// </summary>
        private async void ChargerMenu(int menuId)
        {
            try
            {
                if (App.CurrentRestaurant != null)
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);

                MenuModel? menu = await App.MenuRepository.GetByIdAsync(menuId);

                if (menu != null)
                {
                    _menuBrouillonId = menu.Id;
                    txtNomMenu.Text = menu.Nom;

                    ElementMenu? amuseBouche = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.AmuseBouche);
                    cmbAmuseGueule.SelectedValue = amuseBouche?.PlatId;

                    ElementMenu? boissonAperitif = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.BoissonAperitif);
                    cmbBoissonAperitif.SelectedValue = boissonAperitif?.PlatId;

                    ElementMenu? entree = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Entree);
                    cmbEntree.SelectedValue = entree?.PlatId;

                    ElementMenu? platPrincipal = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.PlatPrincipal);
                    cmbPlat.SelectedValue = platPrincipal?.PlatId;

                    ElementMenu? vin = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Vin);
                    cmbVin.SelectedValue = vin?.PlatId;

                    ElementMenu? fromage = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Fromage);
                    cmbFromage.SelectedValue = fromage?.PlatId;

                    ElementMenu? dessert = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Dessert);
                    cmbDessert.SelectedValue = dessert?.PlatId;

                    btnSupprimer.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Menu introuvable", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    _menuBrouillonId = null;
                    btnSupprimer.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du menu : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                _menuBrouillonId = null;
                btnSupprimer.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Charge le brouillon du menu
        /// </summary>
        private async void ChargerBrouillon()
        {
            try
            {
                if (App.CurrentRestaurant != null)
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);

                MenuModel? menu = await App.MenuRepository.GetBrouillonAsync();

                if (menu != null)
                {
                    _menuBrouillonId = menu.Id;
                    txtNomMenu.Text = menu.Nom;

                    ElementMenu? amuseBouche = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.AmuseBouche);
                    cmbAmuseGueule.SelectedValue = amuseBouche?.PlatId;

                    ElementMenu? boissonAperitif = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.BoissonAperitif);
                    cmbBoissonAperitif.SelectedValue = boissonAperitif?.PlatId;

                    ElementMenu? entree = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Entree);
                    cmbEntree.SelectedValue = entree?.PlatId;

                    ElementMenu? platPrincipal = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.PlatPrincipal);
                    cmbPlat.SelectedValue = platPrincipal?.PlatId;

                    ElementMenu? vin = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Vin);
                    cmbVin.SelectedValue = vin?.PlatId;

                    ElementMenu? fromage = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Fromage);
                    cmbFromage.SelectedValue = fromage?.PlatId;

                    ElementMenu? dessert = menu.Elements.FirstOrDefault(e => e.Categorie == CategoriePlat.Dessert);
                    cmbDessert.SelectedValue = dessert?.PlatId;

                    btnSupprimer.Visibility = Visibility.Visible;
                }
                else
                {
                    _menuBrouillonId = null;
                    btnSupprimer.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                _menuBrouillonId = null;
                btnSupprimer.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Enregistre le menu
        /// </summary>
        private async void EnregistrerMenu(string statut, bool estValidation)
        {
            btnAnnuler.IsEnabled = false;
            btnEnregistrerBrouillon.IsEnabled = false;
            btnValider.IsEnabled = false;

            try
            {
                if (App.CurrentRestaurant != null)
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);

                MenuModel menu = ConstruireMenu(statut);
                bool creation = !_menuBrouillonId.HasValue;
                bool success;

                if (creation)
                {
                    try
                    {
                        MenuModel menuCree = await App.MenuRepository.CreateAsync(menu);
                        _menuBrouillonId = menuCree.Id;
                        success = true;
                    }
                    catch
                    {
                        try
                        {
                            MenuModel? brouillon = await App.MenuRepository.GetBrouillonAsync();
                            if (brouillon != null)
                                _menuBrouillonId = brouillon.Id;
                        }
                        catch { }
                        success = false;
                    }
                }
                else
                {
                    menu.Id = _menuBrouillonId!.Value;
                    success = await App.MenuRepository.UpdateAsync(menu);
                }

                if (success)
                {
                    if (_menuBrouillonId == null && !estValidation)
                    {
                        MessageBox.Show("Brouillon enregistré mais impossible de récupérer son identifiant.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    if (statut.Equals("Validé", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Menu validé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        _menuBrouillonId = null;
                        ReinitialiserSelection();
                    }
                    else
                    {
                        MessageBox.Show("Brouillon enregistré avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    ChargerBrouillon();
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'enregistrement du menu.", "Erreur détaillée", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de l'enregistrement :\n\n" + ex.Message +
                    "\n\nInner: " + (ex.InnerException != null ? ex.InnerException.Message : "Aucune"),
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnAnnuler.IsEnabled = true;
                btnEnregistrerBrouillon.IsEnabled = true;
                btnValider.IsEnabled = true;
                this.Close();
            }
        }

        /// <summary>
        /// Construit un menu à partir des sélections de l'utilisateur
        /// </summary>
        private MenuModel ConstruireMenu(string statut)
        {
            MenuModel menu = new MenuModel();
            menu.Nom = string.IsNullOrWhiteSpace(txtNomMenu.Text) ? "Nouveau menu" : txtNomMenu.Text.Trim();
            menu.Date = DateTime.Now;
            menu.Statut = statut;
            menu.Elements.Clear();

            Plat? amuseBouche = ObtenirPlatSelectionne(cmbAmuseGueule);
            if (amuseBouche != null)
                menu.Elements.Add(new ElementMenu { PlatId = amuseBouche.Id, Plat = amuseBouche, Categorie = CategoriePlat.AmuseBouche, Ordre = 1 });

            Plat? boissonAperitif = ObtenirPlatSelectionne(cmbBoissonAperitif);
            if (boissonAperitif != null)
                menu.Elements.Add(new ElementMenu { PlatId = boissonAperitif.Id, Plat = boissonAperitif, Categorie = CategoriePlat.BoissonAperitif, Ordre = 1 });

            Plat? entree = ObtenirPlatSelectionne(cmbEntree);
            if (entree != null)
                menu.Elements.Add(new ElementMenu { PlatId = entree.Id, Plat = entree, Categorie = CategoriePlat.Entree, Ordre = 1 });

            Plat? platPrincipal = ObtenirPlatSelectionne(cmbPlat);
            if (platPrincipal != null)
                menu.Elements.Add(new ElementMenu { PlatId = platPrincipal.Id, Plat = platPrincipal, Categorie = CategoriePlat.PlatPrincipal, Ordre = 1 });

            Plat? vin = ObtenirPlatSelectionne(cmbVin);
            if (vin != null)
                menu.Elements.Add(new ElementMenu { PlatId = vin.Id, Plat = vin, Categorie = CategoriePlat.Vin, Ordre = 1 });

            Plat? fromage = ObtenirPlatSelectionne(cmbFromage);
            if (fromage != null)
                menu.Elements.Add(new ElementMenu { PlatId = fromage.Id, Plat = fromage, Categorie = CategoriePlat.Fromage, Ordre = 1 });

            Plat? dessert = ObtenirPlatSelectionne(cmbDessert);
            if (dessert != null)
                menu.Elements.Add(new ElementMenu { PlatId = dessert.Id, Plat = dessert, Categorie = CategoriePlat.Dessert, Ordre = 1 });

            return menu;
        }

        /// <summary>
        /// Obtient le plat sélectionné dans une ComboBox
        /// </summary>
        private static Plat? ObtenirPlatSelectionne(ComboBox comboBox) => comboBox.SelectedItem as Plat;

        /// <summary>
        /// Reinitialise la sélection des plats
        /// </summary>
        private void ReinitialiserSelection()
        {
            txtNomMenu.Text = "Nouveau menu";
            cmbAmuseGueule.SelectedIndex = -1;
            cmbBoissonAperitif.SelectedIndex = -1;
            cmbEntree.SelectedIndex = -1;
            cmbPlat.SelectedIndex = -1;
            cmbVin.SelectedIndex = -1;
            cmbFromage.SelectedIndex = -1;
            cmbDessert.SelectedIndex = -1;
        }

        /// <summary>
        /// Supprime le menu/brouillon actuel
        /// </summary>
        private async void SupprimerMenu(object sender, RoutedEventArgs e)
        {
            if (!_menuBrouillonId.HasValue)
            {
                MessageBox.Show("Aucun menu à supprimer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

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

                    if (App.CurrentRestaurant != null)
                        App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);

                    bool success = await App.MenuRepository.DeleteAsync(_menuBrouillonId.Value);

                    if (success)
                    {
                        MessageBox.Show("Menu supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                        _menuBrouillonId = null;
                        ReinitialiserSelection();
                        btnSupprimer.Visibility = Visibility.Collapsed;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la suppression du menu.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        btnSupprimer.IsEnabled = true;
                        btnSupprimer.Content = "Supprimer";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression du menu : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnSupprimer.IsEnabled = true;
                    btnSupprimer.Content = "Supprimer";
                }
            }
        }
    }
}
