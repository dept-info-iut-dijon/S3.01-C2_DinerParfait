using EpicurAPP_Partage.Models;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// Collection des clients
        /// </summary>
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
                List<Client> clients = await App.ClientRepository.GetAllAsync();
                if (clients != null)
                {
                    Clients.Clear();
                    foreach (Client client in clients)
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
                FicheClient ficheClient = new FicheClient(clientSelectionne.Id);
                bool? result = ficheClient.ShowDialog();

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
            FicheClient ficheClient = new FicheClient();
            bool? result = ficheClient.ShowDialog();

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
        /// <exception cref="Exception">Erreur lors de la suppression du client</exception>
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
                bool success = await App.ClientRepository.DeleteAsync(clientSelectionne.Id);

                if (success)
                {
                    //Afficher message "Fiche supprimée avec succès"
                    MessageBox.Show("Client supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Recharger la liste
                    ChargerClients();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression du client.", "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}