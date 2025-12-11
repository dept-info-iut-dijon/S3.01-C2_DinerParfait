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
using System.Text.Json.Nodes;

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

            // Rafraîchir les données quand la vue devient visible
            this.IsVisibleChanged += ReservationsView_IsVisibleChanged;

            // Rafraîchir les menus quand l'utilisateur ouvre la liste déroulante
            cmbMenus.DropDownOpened += CmbMenus_DropDownOpened;
        }

        private async void ReservationsView_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerDonneesInitiales();
        }

        private async Task ChargerDonneesInitiales()
        {
            try
            {
                // S'assurer que le header X-Restaurant-Id est défini
                if (App.CurrentRestaurant != null)
                {
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);
                }

                //Charger les Menus (pour la création de service)
                await ChargerMenusValides();

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

        private async Task ChargerMenusValides()
        {
            try
            {
                if (App.CurrentRestaurant != null)
                {
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);
                }

                var menuSelectionne = cmbMenus.SelectedItem as EpicurAPP_Partage.Models.Menu;
                int? menuSelectionneId = menuSelectionne?.Id;

                var menus = await App.ApiClient.HttpClient.GetFromJsonAsync<List<EpicurAPP_Partage.Models.Menu>>("api/menu");

                if (menus != null)
                {
                    ListeMenus = menus;
                    cmbMenus.ItemsSource = ListeMenus;

                    // Essayer de réappliquer la sélection précédente
                    if (menuSelectionneId.HasValue)
                    {
                        var menuToReselect = ListeMenus.Find(m => m.Id == menuSelectionneId.Value);
                        if (menuToReselect != null) cmbMenus.SelectedItem = menuToReselect;
                        else cmbMenus.SelectedIndex = ListeMenus.Count > 0 ? 0 : -1;
                    }
                    else
                    {
                        cmbMenus.SelectedIndex = ListeMenus.Count > 0 ? 0 : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des menus : {ex.Message}");
            }
        }

        private async void ReservationsView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Recharger les données quand la vue devient visible
            if (this.IsVisible)
            {
                await ChargerMenusValides();
                await ChargerServices();
            }
        }

        private async void CmbMenus_DropDownOpened(object sender, EventArgs e)
        {
            // Recharger les menus quand l'utilisateur ouvre la liste
            await ChargerMenusValides();
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

                PanelReservations.IsEnabled = true;
                PanelReservations.Opacity = 1;

                if (service.EstVerrouille)
                {
                    // Cas verrouillé 
                    txtServiceSelectionne.Text = $"Service du {service.Date:dd/MM} ({service.MidiSoir}) - VERROUILLÉ 🔒";
                    txtServiceSelectionne.Foreground = System.Windows.Media.Brushes.Red;
                    cmbMenus.IsEnabled = false;
                }
                else
                {
                    // Cas normal 
                    txtServiceSelectionne.Text = $"Service du {service.Date:dd/MM} ({service.MidiSoir})";
                    txtServiceSelectionne.Foreground = System.Windows.Media.Brushes.Black;
                    cmbMenus.IsEnabled = true;
                }
                // ---------------------------------------------------

                await ChargerReservations(service.Id);
            }
            else
            {
                PanelReservations.IsEnabled = false;
                PanelReservations.Opacity = 0.6;
                _serviceSelectionne = null;
            }
        }

        // GESTION RÉSERVATION

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
            var resultat = await detectionService.DetecterConflitAsync(client.Id, _serviceSelectionne.MenuId);

            bool forceOverride = false;
            string? noteOverride = null;


            // Créer la réservation
            var request = new ReservationRequest
            {
                ClientId = client.Id,
                MenuId = _serviceSelectionne.MenuId,
                ServiceId = _serviceSelectionne.Id,
                NbCouverts = nbCouverts,
                ForceReservation = forceOverride,
                NoteOverride = noteOverride
            };

            try
            {
                // Utiliser le bon endpoint qui gère le forçage
                var response = await App.ApiClient.HttpClient.PostAsJsonAsync(
                    "AllergeneDetection/valider-reservation", request);

                if (response.IsSuccessStatusCode)
                {
                    string message = forceOverride
                        ? $"Réservation créée avec override allergène.\nNote : {noteOverride}"
                        : "Réservation créée avec succès !";
                    MessageBox.Show(message, "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    await ChargerReservations(_serviceSelectionne.Id);
                    txtCouverts.Text = "2";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var jsonNode = JsonNode.Parse(jsonString);
                    string messageAlerte = jsonNode["Conflits"]?[0]?["Message"]?.ToString() ?? "Conflit d'allergie détecté.";
                    var choix = MessageBox.Show(
                        $"{messageAlerte}\n\nVoulez-vous forcer la réservation malgré le risque ?",
                        "Alerte Sécurité Alimentaire",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (choix == MessageBoxResult.Yes)
                    {
                        var responseForce = await App.ApiClient.HttpClient.PostAsJsonAsync("Services/Reservation?force=true", request);

                        if (responseForce.IsSuccessStatusCode)
                        {
                            await ChargerReservations(_serviceSelectionne.Id);
                            MessageBox.Show("Réservation forcée avec succès.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Impossible de forcer la réservation.");
                        }
                    }
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