using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace EpicurAppIHM.Views
{
    public partial class ClientsView : UserControl
    {
        private HttpClient _httpClient;

        public ClientsView()
        {
            InitializeComponent();

            // Configurer HttpClient pour appeler l'API
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7068/");

            // Charger les clients quand la vue est affichée
            this.Loaded += ClientsView_Loaded;
        }

        /// <summary>
        /// Événement déclenché quand la vue est chargée
        /// </summary>
        private async void ClientsView_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerClients();
        }

        /// <summary>
        /// Charge la liste des clients depuis l'API
        /// </summary>
        public async System.Threading.Tasks.Task ChargerClients()
        {
            try
            {
                // Appel GET à l'API pour récupérer tous les clients
                var clients = await _httpClient.GetFromJsonAsync<List<ClientDto>>("Client");

                // Afficher dans le DataGrid
                DataGridClients.ItemsSource = clients;
            }
            catch (HttpRequestException)
            {
                DataGridClients.ItemsSource = new List<ClientDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients :\n{ex.Message}",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ouvre le formulaire de création d'un client
        /// </summary>
        private async void OuvrirFicheClient(object sender, RoutedEventArgs e)
        {
            FicheClient ficheClient = new FicheClient();
            ficheClient.ShowDialog();

            // IMPORTANT : Recharger les clients après création
            await ChargerClients();
        }
    }

    /// <summary>
    /// Classe représentant un client (correspond à ce que l'API renvoie)
    /// </summary>
    public class ClientDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Allergies { get; set; }
        public string? Notes { get; set; }

        /// <summary>
        /// Transforme la chaîne d'allergies en liste pour l'affichage
        /// </summary>
        public List<string> AllergiesList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Allergies))
                    return new List<string>();

                return Allergies.Split(',')
                                .Select(a => a.Trim())
                                .Where(a => !string.IsNullOrEmpty(a))
                                .ToList();
            }
        }
    }
}