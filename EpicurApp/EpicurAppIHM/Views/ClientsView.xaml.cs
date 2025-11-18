using EpicurAPP_Partage.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Page d'affichage client
    /// </summary>
    public partial class ClientsView : UserControl
    {
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();

        //Client HTTP local
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Intancie la page d'affichage client
        /// </summary>

        public ClientsView()
        {
            InitializeComponent();

            // Utilisation du service ApiClient centralisé
            _httpClient = App.ApiClient.HttpClient;
            
            DataGridClients.ItemsSource = Clients;
            ChargerClients();
        }

        /// <summary>
        /// Charge les clients
        /// </summary>
        /// <exception cref="Exception">Erreur du chargement client</exception>
        public async void ChargerClients()
        {
            try
            {
                var clients = await _httpClient.GetFromJsonAsync<List<Client>>("Client");
                if (clients != null)
                {
                    Clients.Clear();
                    foreach (var client in clients)
                        Clients.Add(client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients : {ex.Message}",
                                "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Affiche une fiche client
        /// </summary>
        private void DataGridClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridClients.SelectedItem is Client clientSelectionne)
            {
                var ficheClient = new FicheClient(clientSelectionne.Id);
                var result = ficheClient.ShowDialog();

                if (result == true)
                {
                    ChargerClients();
                }
            }
        }

        /// <summary>
        /// Affiche la fiche de creation client
        /// </summary>
        private void NouveauClient_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fiche client en mode création
            var ficheClient = new FicheClient();
            var result = ficheClient.ShowDialog();

            // Recharger la liste si un client a été créé
            if (result == true)
            {
                ChargerClients();
            }
        }

        /// <summary>
        /// Gère la suppression du client sélectionné.
        /// (Appelée par le bouton "Supprimer le client sélectionné")
        /// </summary>
        private async void SupprimerClient_Click(object sender, RoutedEventArgs e)
        {
            //Vérifier si un client est sélectionné
            if (DataGridClients.SelectedItem is not Client clientSelectionne)
            {
                MessageBox.Show("Veuillez sélectionner un client dans la liste à supprimer.","Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Demander confirmation
            MessageBoxResult confirmation = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le client {clientSelectionne.Prenom} {clientSelectionne.Nom} ?\nCette action est irréversible.","Confirmation de suppression",MessageBoxButton.YesNo,MessageBoxImage.Warning);

            if (confirmation == MessageBoxResult.No)
            {
                return;
            }

            //Appel à l'API
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"Client/{clientSelectionne.Id}");

                if (response.IsSuccessStatusCode)
                {
                    //Afficher message "Fiche supprimée avec succès"
                    MessageBox.Show("Client supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Recharger la liste
                    ChargerClients();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erreur lors de la suppression : {errorContent}", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}