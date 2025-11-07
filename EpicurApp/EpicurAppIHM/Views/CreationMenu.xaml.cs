using EpicurApp_API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    public partial class CreationMenu : Window
    {
        private HttpClient _httpClient;
        private List<Plat> tousLesPlats;

        public CreationMenu()
        {
            InitializeComponent();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7068/");

            ChargerPlats();

            btnAnnuler.Click += Annuler;
            btnCreer.Click += CreerMenu;
        }

        private void ChargerPlats()
        {
            try
            {
                HttpResponseMessage response = _httpClient.GetAsync("plats").Result;
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
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée (https://localhost:7068)", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        private void ConfigurerComboBox(ComboBox comboBox, string categorie)
        {
            List<Plat> platsClasse = new List<Plat>();

            foreach (Plat plat in tousLesPlats)
            {
                if (plat.Categorie == categorie)
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

        private void Annuler(object sender, RoutedEventArgs e)
        {
            cmbAmuseGueule.SelectedIndex = -1;
            cmbBoissonAperitif.SelectedIndex = -1;
            cmbEntree.SelectedIndex = -1;
            cmbPlat.SelectedIndex = -1;
            cmbVin.SelectedIndex = -1;
            cmbFromage.SelectedIndex = -1;
            cmbDessert.SelectedIndex = -1;
        }

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

        private void CreerMenu(object sender, RoutedEventArgs e)
        {
            if (!ValidationMenu()) return;

            btnCreer.IsEnabled = false;
            btnCreer.Content = "Création en cours...";

            try
            {
                EpicurApp_API.Models.Menu menu = new EpicurApp_API.Models.Menu();
                menu.Nom = "Nouveau menu";
                menu.Date = DateTime.Now;
                menu.Statut = "Brouillon";

                menu.AmuseBoucheId = cmbAmuseGueule.SelectedValue as int?;
                menu.BoissonAperitifId = cmbBoissonAperitif.SelectedValue as int?;
                menu.EntreeId = cmbEntree.SelectedValue as int?;
                menu.PlatPrincipalId = cmbPlat.SelectedValue as int?;
                menu.VinId = cmbVin.SelectedValue as int?;
                menu.FromageId = cmbFromage.SelectedValue as int?;
                menu.DessertId = cmbDessert.SelectedValue as int?;

                menu.AmuseBouche = null;
                menu.BoissonAperitif = null;
                menu.Entree = null;
                menu.PlatPrincipal = null;
                menu.Vin = null;
                menu.Fromage = null;
                menu.Dessert = null;

                HttpResponseMessage response = _httpClient.PostAsJsonAsync("Menu", menu).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Menu créé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    Annuler(sender, e);
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
                    "Exception lors de la création :\n\n" + ex.Message +
                    "\n\nInner: " + (ex.InnerException != null ? ex.InnerException.Message : "Aucune"),
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btnCreer.IsEnabled = true;
                btnCreer.Content = "Créer le Menu";
            }
        }
    }
}
