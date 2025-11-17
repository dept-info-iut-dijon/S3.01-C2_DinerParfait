using EpicurAPP_Partage.Models;
using EpicurAppIHM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MenuModel = EpicurAPP_Partage.Models.Menu;

namespace EpicurAppIHM.Views
{
    public partial class MenusView : UserControl
    {
        public ObservableCollection<MenuModel> Menus { get; set; } = new ObservableCollection<MenuModel>();

        public MenusView()
        {
            InitializeComponent();
            ListBoxMenus.ItemsSource = Menus;
            ChargerMenus();
        }

        public async void ChargerMenus()
        {
            try
            {
                var menus = await App.ApiClient.HttpClient.GetFromJsonAsync<List<MenuModel>>("Menu/GetAll");
                if (menus != null)
                {
                    Menus.Clear();
                    foreach (var menu in menus)
                        Menus.Add(menu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des menus : {ex.Message}",
                                "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBoxMenus_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxMenus.SelectedItem is MenuModel menuSelectionne)
            {
                // Ouvrir la fenêtre de consultation du menu (lecture seule)
                var ficheMenu = new ConsultationMenu(menuSelectionne.Id);
                ficheMenu.ShowDialog();

                // Recharger la liste après fermeture
                ChargerMenus();
            }
        }

        private void NouveauMenu_Click(object sender, RoutedEventArgs e)
        {
            var creationMenu = new CreationMenu();
            creationMenu.Closed += (s, args) => ChargerMenus(); // Recharger quand la fenêtre se ferme
            creationMenu.ShowDialog();
        }
    }
}