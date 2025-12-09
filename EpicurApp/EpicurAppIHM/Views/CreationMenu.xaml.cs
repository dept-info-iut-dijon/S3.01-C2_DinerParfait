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
            dpDateMenu.SelectedDateChanged += (s, e) => VerifierVerrouillageDate();
        }

        /// <summary>
        /// Charge les plats
        /// </summary>
        /// <exception cref="Exception">API introuvable par le client</exception>
        private async void ChargerPlats()
        {
            try
            {
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
            catch
            {
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée (ex: https://localhost:8081)", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// <param name="categorieStr">catégorie du pla</param>
        private void ConfigurerComboBox(ComboBox comboBox, string categorieStr)
        {
            List<Plat> platsClasse = new List<Plat>();

            // Convertir la string en enum CategoriePlat
            if (!Enum.TryParse<CategoriePlat>(categorieStr, out CategoriePlat categorieEnum))
            {
                return;
            }

            foreach (Plat plat in tousLesPlats)
            {
                if (plat.Categorie == categorieEnum)
                {
                    platsClasse.Add(plat);
                }
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
        /// <param name="p1">1er plat a comparer</param>
        /// <param name="p2">2eme plat a comparer</param>
        /// <returns>renvoie le plat qui est le plus "grand"</returns>
        private int ComparerPlatsParNom(Plat p1, Plat p2)
        {
            int res;
            if (p1 == null && p2 == null) res = 0;
            if (p1 == null) res = -1;
            if (p2 == null) res = 1;

            if (p1.Nom == null && p2.Nom == null) res = 0;
            if (p1.Nom == null) res = -1;
            if (p2.Nom == null) res = 1;

            res = p1.Nom.CompareTo(p2.Nom);
            return res;
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
        /// <returns>True si au moins un plat est selectionnée sinon faux</returns>
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
        /// <param name="menuId">ID du menu à charger</param>
        /// <exception cref="Exception">Erreur lors de l'appel API</exception>
        private async Task ChargerMenu(int menuId)
        {
            try
            {
                // GET direct
                HttpResponseMessage response = await _httpClient.GetAsync($"Menu/{menuId}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show("Menu introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    _menuBrouillonId = null;
                    dpDateMenu.SelectedDate = DateTime.Today;
                    btnSupprimer.Visibility = Visibility.Collapsed;
                    AppliquerVerrouillageUI(false);
                    return;
                }

                response.EnsureSuccessStatusCode();
                MenuModel menu = await response.Content.ReadFromJsonAsync<MenuModel>();

                if (menu != null)
                {
                    MettreAJourUI(menu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement menu : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Charge le brouillon du menu
        /// </summary>
        /// <exception cref="Exception">Erreur lors de l'appel API</exception>
        private async Task ChargerBrouillon()
        {
            try
            {
                // GET direct
                HttpResponseMessage response = await _httpClient.GetAsync("Menu/Brouillon");

                if (response.IsSuccessStatusCode)
                {
                    MenuModel menu = await response.Content.ReadFromJsonAsync<MenuModel>();
                    if (menu != null)
                    {
                        MettreAJourUI(menu);
                        return;
                    }
                }

                // Si pas de brouillon ou erreur 404
                _menuBrouillonId = null;
                dpDateMenu.SelectedDate = DateTime.Today;
                btnSupprimer.Visibility = Visibility.Collapsed;
                AppliquerVerrouillageUI(false);
            }
            catch
            {
                // Erreur silencieuse pour le brouillon
                _menuBrouillonId = null;
                btnSupprimer.Visibility = Visibility.Collapsed;
                AppliquerVerrouillageUI(false);
            }
        }

        /// <summary>
        /// Met à jour l'interface utilisateur avec les données du menu
        /// </summary>
        /// <param name="menu">menu mis a jour</param>
        private void MettreAJourUI(MenuModel menu)
        {
            _menuBrouillonId = menu.Id;
            txtNomMenu.Text = menu.Nom;
            dpDateMenu.SelectedDate = menu.Date;

            cmbAmuseGueule.SelectedValue = menu.AmuseBouche?.Id;
            cmbBoissonAperitif.SelectedValue = menu.BoissonAperitif?.Id;
            cmbEntree.SelectedValue = menu.Entree?.Id;
            cmbPlat.SelectedValue = menu.PlatPrincipal?.Id;
            cmbVin.SelectedValue = menu.Vin?.Id;
            cmbFromage.SelectedValue = menu.Fromage?.Id;
            cmbDessert.SelectedValue = menu.Dessert?.Id;

            btnSupprimer.Visibility = Visibility.Visible;

            // Application du verrouillage 48h
            AppliquerVerrouillageUI(menu.EstVerrouille);
        }

        /// <summary>
        /// Enregistre le menu 
        /// </summary>
        /// <param name="statut">menu brouillon ou validé</param>
        /// <param name="estValidation">Si le menu va etre enregistré en mode validé</param>
        /// <exception cref="Exception">Erreur lors de l'enregistrement</exception>
        private async void EnregistrerMenu(string statut, bool estValidation)
        {
            btnAnnuler.IsEnabled = false;
            btnEnregistrerBrouillon.IsEnabled = false;
            btnValider.IsEnabled = false;

            try
            {
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
                        // Si la création échoue, essayer de récupérer le brouillon
                        try
                        {
                            MenuModel? brouillon = await App.MenuRepository.GetBrouillonAsync();
                            if (brouillon != null)
                            {
                                _menuBrouillonId = brouillon.Id;
                            }
                        }
                        catch
                        {
                        }
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
                        MessageBox.Show("Brouillon enregistré mais impossible de récupérer son identifiant.",
                                        "Avertissement",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
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
                    MessageBox.Show("Erreur lors de l'enregistrement du menu.",
                        "Erreur détaillée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erreur lors de l'enregistrement :\n\n" + ex.Message +
                    "\n\nInner: " + (ex.InnerException != null ? ex.InnerException.Message : "Aucune"),
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
        /// <param name="statut">si le menu est validé ou brouillon</param>
        /// <returns>Le menu qui a été construit</returns>
        private MenuModel ConstruireMenu(string statut)
        {
            MenuModel menu = new MenuModel();
            menu.Nom = string.IsNullOrWhiteSpace(txtNomMenu.Text) ? "Nouveau menu" : txtNomMenu.Text.Trim();
            menu.Date = DateTime.Now;
            menu.Statut = statut;

            // Vider la liste des éléments
            menu.Elements.Clear();

            // Récupérer les plats sélectionnés depuis les ComboBox et les ajouter au menu
            Plat? amuseBouche = ObtenirPlatSelectionne(cmbAmuseGueule);
            if (amuseBouche != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = amuseBouche.Id,
                    Plat = amuseBouche,
                    Categorie = CategoriePlat.AmuseBouche,
                    Ordre = 1
                });
            }

            Plat? boissonAperitif = ObtenirPlatSelectionne(cmbBoissonAperitif);
            if (boissonAperitif != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = boissonAperitif.Id,
                    Plat = boissonAperitif,
                    Categorie = CategoriePlat.BoissonAperitif,
                    Ordre = 1
                });
            }

            Plat? entree = ObtenirPlatSelectionne(cmbEntree);
            if (entree != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = entree.Id,
                    Plat = entree,
                    Categorie = CategoriePlat.Entree,
                    Ordre = 1
                });
            }

            Plat? platPrincipal = ObtenirPlatSelectionne(cmbPlat);
            if (platPrincipal != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = platPrincipal.Id,
                    Plat = platPrincipal,
                    Categorie = CategoriePlat.PlatPrincipal,
                    Ordre = 1
                });
            }

            Plat? vin = ObtenirPlatSelectionne(cmbVin);
            if (vin != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = vin.Id,
                    Plat = vin,
                    Categorie = CategoriePlat.Vin,
                    Ordre = 1
                });
            }

            Plat? fromage = ObtenirPlatSelectionne(cmbFromage);
            if (fromage != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = fromage.Id,
                    Plat = fromage,
                    Categorie = CategoriePlat.Fromage,
                    Ordre = 1
                });
            }

            Plat? dessert = ObtenirPlatSelectionne(cmbDessert);
            if (dessert != null)
            {
                menu.Elements.Add(new ElementMenu
                {
                    PlatId = dessert.Id,
                    Plat = dessert,
                    Categorie = CategoriePlat.Dessert,
                    Ordre = 1
                });
            }

            return menu;
        }

        /// <summary>
        /// Obtient le plat sélectionné dans une ComboBox
        /// </summary>
        /// <param name="comboBox">Combobox cible</param>
        /// <returns>Le plat séléctionné</returns>
        private static Plat? ObtenirPlatSelectionne(ComboBox comboBox)
        {
            return comboBox.SelectedItem as Plat;
        }

        

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
        /// <exception cref="Exception">Erreur lors de la suppression</exception>
        private async void SupprimerMenu(object sender, RoutedEventArgs e)
        {
            if (!_menuBrouillonId.HasValue)
            {
                MessageBox.Show("Aucun menu à supprimer.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
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

                    bool success = await App.MenuRepository.DeleteAsync(_menuBrouillonId.Value);

                    if (success)
                    {
                        MessageBox.Show("Menu supprimé avec succès.", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        _menuBrouillonId = null;
                        ReinitialiserSelection();
                        btnSupprimer.Visibility = Visibility.Collapsed;
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

        private void VerifierVerrouillageDate()
        {
            if (dpDateMenu.SelectedDate.HasValue && _menuBrouillonId.HasValue)
            {
                // Vérification dynamique locale pour feedback immédiat
                bool verrouille = (dpDateMenu.SelectedDate.Value - DateTime.Now).TotalHours < 48;
                AppliquerVerrouillageUI(verrouille);
            }
        }

        private void AppliquerVerrouillageUI(bool verrouille)
        {
            // Affiche le bandeau rouge
            bdAlerteVerrouillage.Visibility = verrouille ? Visibility.Visible : Visibility.Collapsed;

            // Grise les champs d'édition
            txtNomMenu.IsEnabled = !verrouille;
            cmbAmuseGueule.IsEnabled = !verrouille;
            cmbBoissonAperitif.IsEnabled = !verrouille;
            cmbEntree.IsEnabled = !verrouille;
            cmbPlat.IsEnabled = !verrouille;
            cmbVin.IsEnabled = !verrouille;
            cmbFromage.IsEnabled = !verrouille;
            cmbDessert.IsEnabled = !verrouille;

            // Cache les boutons de sauvegarde
            btnValider.Visibility = verrouille ? Visibility.Collapsed : Visibility.Visible;
            btnEnregistrerBrouillon.Visibility = verrouille ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
