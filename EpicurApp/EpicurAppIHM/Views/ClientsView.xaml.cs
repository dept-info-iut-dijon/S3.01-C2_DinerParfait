using EpicurAPP_Partage.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Classe pour afficher un client avec son statut inactif
    /// </summary>
    public class ClientAffichage
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Preferences { get; set; }
        public List<Allergene> Allergenes { get; set; }
        public bool EstInactif { get; set; }
        public string IconeStatut => EstVIP ? "*" : (EstInactif ? "!" : "");

        public string IconeInactif
        {
            get { return EstInactif ? "!" : ""; }
        }

        public bool EstVIP;
        
    }

    /// <summary>
    /// Page d'affichage client
    /// </summary>
    public partial class ClientsView : UserControl
    {
        // Client HTTP local
        private readonly HttpClient _httpClient;

        // Liste de tous les clients pour le filtre
        private List<ClientAffichage> tousLesClientsAffichage;

        // Liste des IDs des clients inactifs
        private List<int> idsClientsInactifs;

        /// <summary>
        /// Collection des clients
        /// </summary>
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();

        /// <summary>
        /// Instancie la page d'affichage client
        /// </summary>
        public ClientsView()
        {
            InitializeComponent();

            // Utilisation du service ApiClient centralisé
            _httpClient = App.ApiClient.HttpClient;

            tousLesClientsAffichage = new List<ClientAffichage>();
            idsClientsInactifs = new List<int>();

            ChargerClients();
        }

        /// <summary>
        /// Charge les clients depuis l'API
        /// </summary>
        public async void ChargerClients()
        {
            try
            {
                // Récupérer tous les clients
                List<Client> clients = await _httpClient.GetFromJsonAsync<List<Client>>("Client");
                // Récupérer les clients inactifs
                List<Client> clientsInactifs = await _httpClient.GetFromJsonAsync<List<Client>>("Client/Inactifs");
                // Récupérer les clients VIP
                List<Client> clientsVIP = await _httpClient.GetFromJsonAsync<List<Client>>("Client/VIP");

                // Créer la liste des IDs inactifs
                idsClientsInactifs = new List<int>();
                if (clientsInactifs != null)
                {
                    foreach (Client c in clientsInactifs)
                    {
                        idsClientsInactifs.Add(c.Id);
                    }
                }

                // Créer la liste des IDs VIP
                List<int> idsClientsVIP = new List<int>();
                if (clientsVIP != null)
                {
                    foreach (Client c in clientsVIP)
                    {
                        idsClientsVIP.Add(c.Id);
                    }
                }

                // Créer la liste d'affichage
                tousLesClientsAffichage = new List<ClientAffichage>();
                if (clients != null)
                {
                    foreach (Client client in clients)
                    {
                        ClientAffichage ca = new ClientAffichage
                        {
                            Id = client.Id,
                            Nom = client.Nom,
                            Prenom = client.Prenom,
                            Email = client.Email,
                            Telephone = client.Telephone,
                            Preferences = client.Preferences,
                            Allergenes = client.Allergenes,
                            EstInactif = idsClientsInactifs.Contains(client.Id),
                            EstVIP = idsClientsVIP.Contains(client.Id)
                        };
                        tousLesClientsAffichage.Add(ca);
                    }
                }

                // Afficher avec le filtre
                AppliquerFiltre();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients : {ex.Message}",
                                "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Applique le filtre selon la checkbox
        /// </summary>
        private void AppliquerFiltre()
        {
            if (tousLesClientsAffichage == null) return;

            if (ChkVoirInactifs.IsChecked == true)
            {
                // Afficher seulement les inactifs
                List<ClientAffichage> clientsFiltres = new List<ClientAffichage>();
                foreach (ClientAffichage ca in tousLesClientsAffichage)
                {
                    if (ca.EstInactif)
                    {
                        clientsFiltres.Add(ca);
                    }
                }
                DataGridClients.ItemsSource = clientsFiltres;
            }
            else
            {
                // Afficher tous les clients
                DataGridClients.ItemsSource = tousLesClientsAffichage;
            }
        }

        /// <summary>
        /// coche/décoche la checkbox
        /// </summary>
        private void ChkVoirInactifs_Changed(object sender, RoutedEventArgs e)
        {
            AppliquerFiltre();
        }

        /// <summary>
        /// Affiche une fiche client
        /// </summary>
        private void DataGridClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridClients.SelectedItem is ClientAffichage clientSelectionne)
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
        /// Affiche la fiche de création client
        /// </summary>
        private void NouveauClient_Click(object sender, RoutedEventArgs e)
        {
            FicheClient ficheClient = new FicheClient();
            bool? result = ficheClient.ShowDialog();

            if (result == true)
            {
                ChargerClients();
            }
        }

        /// <summary>
        /// Gère la supp du client sélectionné
        /// </summary>
        private async void SupprimerClient_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridClients.SelectedItem is not ClientAffichage clientSelectionne)
            {
                MessageBox.Show("Veuillez sélectionner un client dans la liste à supprimer.",
                                "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer le client {clientSelectionne.Prenom} {clientSelectionne.Nom} ?\nCette action est irréversible.",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                bool success = await App.ClientRepository.DeleteAsync(clientSelectionne.Id);

                if (success)
                {
                    MessageBox.Show("Client supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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

        /// <summary>
        /// Retour à la page d'accueil (Dashboard)
        /// </summary>
        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is MainView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is MainView mainView)
            {
                mainView.AfficherDashboard();
            }
        }
    }
}