using EpicurAPP_Partage.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Logique d'interaction pour HistoriqueRepas.xaml
    /// </summary>
    public partial class HistoriqueRepas : Window
    {
        /// <summary>
        /// L'instance HttpClient pour les appels API
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// L'ID du client pour lequel afficher l'historique
        /// </summary>
        private int _clientId;

        /// <summary>
        /// Initialise la fenêtre d'historique des repas pour un client donné
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="nomClient">Nom complet du client pour l'affichage</param>
        public HistoriqueRepas(int clientId, string nomClient)
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            _clientId = clientId;

            // Mise à jour du titre avec le nom du client
            txtTitreClient.Text = $"Historique des repas - {nomClient}";

            // Chargement de l'historique
            ChargerHistoriqueRepas();
        }

        /// <summary>
        /// Charge l'historique des repas depuis l'API
        /// </summary>
        private async void ChargerHistoriqueRepas()
        {
            try
            {
                // Appel à l'API pour récupérer l'historique
                var response = await _httpClient.GetAsync($"Client/{_clientId}/repas");

                if (response.IsSuccessStatusCode)
                {
                    // Lecture du contenu de la réponse
                    string content = await response.Content.ReadAsStringAsync();

                    // Vérifier si c'est un message d'absence de repas
                    if (content.Contains("Aucun repas enregistré"))
                    {
                        AfficherAucunRepas();
                    }
                    else
                    {
                        // Désérialiser la liste des repas
                        var repas = await response.Content.ReadFromJsonAsync<List<Repas>>();

                        if (repas == null || repas.Count == 0)
                        {
                            AfficherAucunRepas();
                        }
                        else
                        {
                            // Afficher les repas dans le DataGrid
                            dgRepas.ItemsSource = repas;
                            dgRepas.Visibility = Visibility.Visible;
                            txtAucunRepas.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Erreur lors du chargement de l'historique : {response.StatusCode}",
                                   "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    AfficherAucunRepas();
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Impossible de contacter l'API.\nVérifiez qu'elle est bien lancée.",
                               "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                AfficherAucunRepas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'historique : {ex.Message}",
                               "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                AfficherAucunRepas();
            }
        }

        /// <summary>
        /// Affiche le message "Aucun repas enregistré" et cache le DataGrid
        /// </summary>
        private void AfficherAucunRepas()
        {
            dgRepas.Visibility = Visibility.Collapsed;
            txtAucunRepas.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Ferme la fenêtre
        /// </summary>
        private void Fermer(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
