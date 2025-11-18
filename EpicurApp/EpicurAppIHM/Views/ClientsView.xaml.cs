using EpicurAPP_Partage.Models;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Intancie la page d'affichage client
        /// </summary>
        public ClientsView()
        {
            InitializeComponent();
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
                var clients = await App.ApiClient.HttpClient.GetFromJsonAsync<List<Client>>("Client");
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
                // Ouvrir la fiche client en mode consultation (lecture seule)
                var ficheClient = new FicheClient(clientSelectionne.Id, modeConsultation: true);
                ficheClient.ShowDialog();

                // Recharger la liste après fermeture
                ChargerClients();
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
    }
}