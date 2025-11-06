using EpicurApp_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    public partial class CreationMenu : Window
    {
        private readonly HttpClient _httpClient;
        private List<Plat> tousLesPlats;

        public CreationMenu()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7068/")
            };

            ChargerPlats();

            btnAnnuler.Click += Annuler;
            btnCreer.Click += CreerMenu;
        }

        private void ChargerPlats()
        {
            try
            {
                var response = _httpClient.GetAsync("api/plats").Result;
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
            comboBox.ItemsSource = tousLesPlats
                .Where(p => p.Categorie == categorie)
                .OrderBy(p => p.Nom)
                .ToList();

            comboBox.DisplayMemberPath = "Nom"; 
            comboBox.SelectedValuePath = "Id"; // ce que SelectedValue renverra
            comboBox.SelectedIndex = -1;
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
                var menuData = new
                {
                    nom = "Nouveau menu",
                    date = DateTime.Now,
                    statut = "Brouillon",
                    amuseBoucheId = cmbAmuseGueule.SelectedValue as int?,
                    boissonAperitifId = cmbBoissonAperitif.SelectedValue as int?,
                    entreeId = cmbEntree.SelectedValue as int?,
                    platPrincipalId = cmbPlat.SelectedValue as int?,
                    vinId = cmbVin.SelectedValue as int?,
                    fromageId = cmbFromage.SelectedValue as int?,
                    dessertId = cmbDessert.SelectedValue as int?,

                    amuseBouche = (object)null,
                    boissonAperitif = (object)null,
                    entree = (object)null,
                    platPrincipal = (object)null,
                    vin = (object)null,
                    fromage = (object)null,
                    dessert = (object)null
                };

                var response = _httpClient.PostAsJsonAsync("Menu", menuData).Result;

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Menu créé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    Annuler(sender, e);
                }
                else
                {
                    string errorDetails = response.Content.ReadAsStringAsync().Result;
                    MessageBox.Show($"Erreur {(int)response.StatusCode} ({response.StatusCode}):\n\n{errorDetails}",
                                    "Erreur détaillée",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception lors de la création :\n\n{ex.Message}\n\nInner: {ex.InnerException?.Message}",
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


