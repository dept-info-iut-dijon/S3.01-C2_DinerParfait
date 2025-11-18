using EpicurAPP_Partage.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
        /// Le client http de l'api
        /// </summary>
        private HttpClient _httpClient;
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
        public CreationMenu()
        {
            InitializeComponent();

            _httpClient = App.ApiClient.HttpClient;

            ChargerPlats();
            ChargerBrouillon();

            btnAnnuler.Click += Annuler;
            btnEnregistrerBrouillon.Click += EnregistrerBrouillon;
            btnValider.Click += ValiderMenu;
        }

        /// <summary>
        /// Charge les plats
        /// </summary>
        /// <exception cref="Exception">API introuvable par le client</exception>
        private void ChargerPlats()
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync("Plats").Result;
                response.EnsureSuccessStatusCode();

                tousLesPlats = response.Content.ReadFromJsonAsync<List<Plat>>().Result;

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
        /// Charge le brouillon du menu
        /// </summary>
        private void ChargerBrouillon()
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync("Menu/Brouillon").Result;

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _menuBrouillonId = null;
                    return;
                }

                response.EnsureSuccessStatusCode();

                MenuModel? menu = response.Content.ReadFromJsonAsync<MenuModel>().Result;
                if (menu != null)
                {
                    _menuBrouillonId = menu.Id;

                    cmbAmuseGueule.SelectedValue = menu.AmuseBouche?.Id;
                    cmbBoissonAperitif.SelectedValue = menu.BoissonAperitif?.Id;
                    cmbEntree.SelectedValue = menu.Entree?.Id;
                    cmbPlat.SelectedValue = menu.PlatPrincipal?.Id;
                    cmbVin.SelectedValue = menu.Vin?.Id;
                    cmbFromage.SelectedValue = menu.Fromage?.Id;
                    cmbDessert.SelectedValue = menu.Dessert?.Id;
                }
                else
                {
                    _menuBrouillonId = null;
                }
            }
            catch
            {
                _menuBrouillonId = null;
            }
        }

        /// <summary>
        /// Enregistre le menu 
        /// </summary>
        /// <param name="statut">menu brouillon ou validé</param>
        /// <param name="estValidation">Si le menu va etre enregistré en mode validé</param>
        /// <exception cref="Exception">Erreur lors de l'enregistrement</exception>
        private void EnregistrerMenu(string statut, bool estValidation)
        {
            btnAnnuler.IsEnabled = false;
            btnEnregistrerBrouillon.IsEnabled = false;
            btnValider.IsEnabled = false;

            try
            {
                MenuModel menu = ConstruireMenu(statut);
                HttpResponseMessage response;
                bool creation = !_menuBrouillonId.HasValue;

                if (creation)
                {
                    response = _httpClient.PostAsJsonAsync("Menu", menu).Result;
                }
                else
                {
                    menu.Id = _menuBrouillonId!.Value;
                    response = _httpClient.PutAsJsonAsync($"Menu/{menu.Id}", menu).Result;
                }

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
                {
                    if (creation)
                    {
                        MenuModel? menuCree = null;
                        try
                        {
                            menuCree = response.Content.ReadFromJsonAsync<MenuModel>().Result;
                        }
                        catch
                        {
                            menuCree = null;
                        }

                        if (menuCree != null)
                        {
                            _menuBrouillonId = menuCree.Id;
                        }
                        else
                        {
                            try
                            {
                                HttpResponseMessage brouillonResponse = _httpClient.GetAsync("Menu/Brouillon").Result;
                                if (brouillonResponse.IsSuccessStatusCode)
                                {
                                    MenuModel? brouillon = brouillonResponse.Content.ReadFromJsonAsync<MenuModel>().Result;
                                    if (brouillon != null)
                                    {
                                        _menuBrouillonId = brouillon.Id;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (_menuBrouillonId == null)
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
                    string errorDetails = response.Content.ReadAsStringAsync().Result;
                    MessageBox.Show(
                        "Erreur " + ((int)response.StatusCode) + " (" + response.StatusCode + "):\n\n" + errorDetails,
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
            }
        }

        /// <summary>
        /// Construit un menu à partir des sélections de l'utilisateur
        /// </summary>
        /// <param name="statut">si le menu est validé ou brouillon</param>
        /// <returns></returns>
        private MenuModel ConstruireMenu(string statut)
        {
            MenuModel menu = new MenuModel();
            menu.Nom = "Nouveau menu";
            menu.Date = DateTime.Now;
            menu.Statut = statut;

            // Récupérer les plats sélectionnés depuis les ComboBox
            menu.AmuseBouche = ObtenirPlatSelectionne(cmbAmuseGueule);
            menu.BoissonAperitif = ObtenirPlatSelectionne(cmbBoissonAperitif);
            menu.Entree = ObtenirPlatSelectionne(cmbEntree);
            menu.PlatPrincipal = ObtenirPlatSelectionne(cmbPlat);
            menu.Vin = ObtenirPlatSelectionne(cmbVin);
            menu.Fromage = ObtenirPlatSelectionne(cmbFromage);
            menu.Dessert = ObtenirPlatSelectionne(cmbDessert);

            return menu;
        }

        /// <summary>
        /// Obtient le plat sélectionné dans une ComboBox
        /// </summary>
        /// <param name="comboBox">Combobox cible</param>
        /// <returns></returns>
        private static Plat? ObtenirPlatSelectionne(ComboBox comboBox)
        {
            return comboBox.SelectedItem as Plat;
        }

        /// <summary>
        /// Obtient la valeur sélectionnée dans une ComboBox
        /// </summary>
        /// <param name="comboBox">combobox cible</param>
        /// <returns></returns>
        private static int? ObtenirValeurSelectionnee(ComboBox comboBox)
        {
            if (comboBox.SelectedValue == null)
            {
                return null;
            }

            if (comboBox.SelectedValue is int valeur)
            {
                return valeur;
            }

            if (int.TryParse(comboBox.SelectedValue.ToString(), out int resultat))
            {
                return resultat;
            }

            return null;
        }

        /// <summary>
        /// Reinitialise la sélection des plats
        /// </summary>
        private void ReinitialiserSelection()
        {
            cmbAmuseGueule.SelectedIndex = -1;
            cmbBoissonAperitif.SelectedIndex = -1;
            cmbEntree.SelectedIndex = -1;
            cmbPlat.SelectedIndex = -1;
            cmbVin.SelectedIndex = -1;
            cmbFromage.SelectedIndex = -1;
            cmbDessert.SelectedIndex = -1;
        }
    }
}
