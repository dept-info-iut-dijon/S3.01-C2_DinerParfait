using EpicurAPP_Partage.Models;
using EpicurAppIHM.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Windows;

namespace EpicurAppIHM.Views
{
    public partial class ConsultationMenu : Window
    {
        private int _menuId;

        public ConsultationMenu(int menuId)
        {
            InitializeComponent();
            _menuId = menuId;
            ChargerMenu();
        }

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

        // Méthode pour obtenir le nom d'un plat depuis la liste
        private string ObtenirNomPlat(List<Plat> plats, int? platId)
        {
            if (platId == null || plats == null)
                return "-";

            Plat plat = plats.Find(p => p.Id == platId.Value);
            return plat?.Nom ?? "-";
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
