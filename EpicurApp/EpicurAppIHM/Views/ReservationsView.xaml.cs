using EpicurAPP_Partage.Models;
using EpicurAppIHM.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    public partial class ReservationsView : UserControl
    {
        // Collections pour l'affichage
        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();
        public ObservableCollection<Reservation> Reservations { get; set; } = new ObservableCollection<Reservation>();

        // Listes pour les ComboBox
        public List<EpicurAPP_Partage.Models.Menu> ListeMenus { get; set; } = new List<EpicurAPP_Partage.Models.Menu>();
        public List<Client> ListeClients { get; set; } = new List<Client>();

        private Service _serviceSelectionne;

        public ReservationsView()
        {
            InitializeComponent();

            // Liaison des données aux grilles
            GridServices.ItemsSource = Services;
            GridReservations.ItemsSource = Reservations;

            // Chargement initial
            this.Loaded += ReservationsView_Loaded;
        }

        private async void ReservationsView_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerDonneesInitiales();
        }

        private async Task ChargerDonneesInitiales()
        {
            try
            {
                //Charger les Menus (pour la création de service)
                var menus = await App.ApiClient.HttpClient.GetFromJsonAsync<List<EpicurAPP_Partage.Models.Menu>>("Menu");
                if (menus != null)
                {
                    ListeMenus = menus;
                    cmbMenus.ItemsSource = ListeMenus;
                }

                //Charger les Clients (pour la prise de réservation)
                var clients = await App.ApiClient.HttpClient.GetFromJsonAsync<List<Client>>("Client");
                if (clients != null)
                {
                    ListeClients = clients;
                    cmbClients.ItemsSource = ListeClients;
                }

                //Charger les Services existants
                await ChargerServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement : {ex.Message}");
            }
        }

        private async Task ChargerServices()
        {
            try
            {
                var servicesApi = await App.ApiClient.HttpClient.GetFromJsonAsync<List<Service>>("Services"); // Route du ServicesController
                Services.Clear();
                if (servicesApi != null)
                {
                    foreach (var s in servicesApi) Services.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de charger les services : {ex.Message}");
            }
        }

        // --- GESTION SERVICES ---

        private async void CreerService_Click(object sender, RoutedEventArgs e)
        {
            if (datePickerService.SelectedDate == null || cmbMenus.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une date et un menu.");
                return;
            }

            var nouveauService = new Service
            {
                Id = 0,
                Date = datePickerService.SelectedDate.Value,
                MidiSoir = (cmbMidiSoir.SelectedItem as ComboBoxItem)?.Content.ToString(),
                MenuId = (cmbMenus.SelectedItem as EpicurAPP_Partage.Models.Menu).Id,
                Statut = "Ouvert"
            };

            try
            {
                var response = await App.ApiClient.HttpClient.PostAsJsonAsync("Services", nouveauService);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Service planifié avec succès !");
                    await ChargerServices();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la création du service.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur API : {ex.Message}");
            }
        }

        private async void GridServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridServices.SelectedItem is Service service)
            {
                _serviceSelectionne = service;

                // Active le panneau de droite
                PanelReservations.IsEnabled = true;
                PanelReservations.Opacity = 1;
                txtServiceSelectionne.Text = $"Service du {service.Date:dd/MM} ({service.MidiSoir})";

                // Charge les réservations de ce service
                await ChargerReservations(service.Id);
            }
            else
            {
                PanelReservations.IsEnabled = false;
                PanelReservations.Opacity = 0.6;
                _serviceSelectionne = null;
            }
        }

        // --- GESTION RÉSERVATIONS ---

        private async Task ChargerReservations(int serviceId)
        {
            try
            {
                // Appel API : GET /Services/{id}/Reservations
                var resas = await App.ApiClient.HttpClient.GetFromJsonAsync<List<Reservation>>($"Services/{serviceId}/Reservations");
                Reservations.Clear();
                if (resas != null)
                {
                    foreach (var r in resas) Reservations.Add(r);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des réservations : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AjouterReservation_Click(object sender, RoutedEventArgs e)
        {
            if (_serviceSelectionne == null || cmbClients.SelectedItem == null) return;

            if (!int.TryParse(txtCouverts.Text, out int nbCouverts) || nbCouverts < 1)
            {
                MessageBox.Show("Nombre de couverts invalide.");
                return;
            }

            var client = cmbClients.SelectedItem as Client;
            if (client == null) return;

            // Vérification des conflits d'allergènes
            var detectionService = new AllergeneDetectionService(App.ApiClient.HttpClient);
            var conflit = await detectionService.DetecterConflitAsync(client.Id, _serviceSelectionne.MenuId);

            bool forceOverride = false;
            string? noteOverride = null;

            if (conflit != null && conflit.AllergenesEnConflit != null && conflit.AllergenesEnConflit.Count > 0)
            {
                // Afficher la popup d'alerte
                var dialog = new AlerteAllergeneDialog(conflit);
                dialog.Owner = Window.GetWindow(this);
                
                if (dialog.ShowDialog() != true)
                {
                    // L'utilisateur a annulé
                    return;
                }

                forceOverride = dialog.ReservationForcee;
                noteOverride = dialog.NoteOverride;
            }

            // Créer la réservation via l'endpoint existant
            var nouvelleResa = new Reservation
            {
                Id = 0,
                ServiceId = _serviceSelectionne.Id,
                ClientId = client.Id,
                NbCouverts = nbCouverts
            };

            try
            {
                // Appel API : POST /Services/Reservation (endpoint existant)
                var response = await App.ApiClient.HttpClient.PostAsJsonAsync("Services/Reservation", nouvelleResa);

                if (response.IsSuccessStatusCode)
                {
                    string message = forceOverride 
                        ? $"Réservation créée avec override allergène.\nNote : {noteOverride}" 
                        : "Réservation créée avec succès !";
                    MessageBox.Show(message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    await ChargerReservations(_serviceSelectionne.Id);
                    txtCouverts.Text = "2";
                }
                else
                {
                    string erreur = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur : {erreur}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur technique : {ex.Message}");
            }
        }
    }
}