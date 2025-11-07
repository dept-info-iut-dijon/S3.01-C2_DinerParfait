using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EpicurAppIHM.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly HttpClient _httpClient;

        public ClientsView()
        {
            InitializeComponent();

            // Configuration de l'accès à l'API
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7068/")
            };

            // Charger les clients au démarrage de la vue
            this.Loaded += ClientsView_Loaded;
        }

        /// <summary>
        /// Charge les clients quand la vue s'affiche
        /// </summary>
        private async void ClientsView_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerClients();
        }

        /// <summary>
        /// Récupère tous les clients depuis l'API et les affiche
        /// </summary>
        public async Task ChargerClients()
        {
            try
            {
                // Appel à l'API pour récupérer la liste des clients
                var clients = await _httpClient.GetFromJsonAsync<List<ClientDto>>("Client");

                // Affichage dans le tableau
                DataGridClients.ItemsSource = clients;
            }
            catch (HttpRequestException)
            {
                // Si l'API est inaccessible, afficher un tableau vide
                DataGridClients.ItemsSource = new List<ClientDto>();
            }
            catch (Exception ex)
            {
                // Erreur inattendue
                MessageBox.Show($"Erreur : {ex.Message}",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ouvre le formulaire de création de client
        /// </summary>
        private async void OuvrirFicheClient(object sender, RoutedEventArgs e)
        {
            FicheClient formulaire = new FicheClient();
            formulaire.ShowDialog();

            // Recharger la liste après fermeture du formulaire
            await ChargerClients();
        }

        /// <summary>
        /// Double-clic sur une ligne pour ouvrir la fiche client
        /// </summary>
        private async void DataGridClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var clientSelectionne = DataGridClients.SelectedItem as ClientDto;

            if (clientSelectionne == null) return;

            // Ouvrir la fiche avec les infos du client
            FicheClient formulaire = new FicheClient(clientSelectionne);
            formulaire.ShowDialog();

            // Recharger après modification
            await ChargerClients();
        }
    }

    /// <summary>
    /// Représente un client (données reçues de l'API)
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
        /// Transforme les allergies (séparées par des virgules) en liste
        /// </summary>
        public List<string> AllergiesList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Allergies))
                {
                    return new List<string>();
                }

                return Allergies.Split(',')
                                .Select(a => a.Trim())
                                .Where(a => !string.IsNullOrEmpty(a))
                                .ToList();
            }
        }
    }
}