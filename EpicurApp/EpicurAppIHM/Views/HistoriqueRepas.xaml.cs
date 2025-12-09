using EpicurAPP_Partage.Models;
using System.Windows;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Logique d'interaction pour HistoriqueRepas.xaml
    /// </summary>
    public partial class HistoriqueRepas : Window
    {
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
            _clientId = clientId;

            // Mise à jour du titre avec le nom du client
            txtTitreClient.Text = $"Historique des repas - {nomClient}";

            // Chargement de l'historique
            ChargerHistoriqueRepas();
        }

        /// <summary>
        /// Charge l'historique des repas depuis l'API
        /// </summary>
        /// <exception cref="Exception">Lancée en cas d'erreur lors de l'appel API</exception>
        private async void ChargerHistoriqueRepas()
        {
            try
            {
                // Appel au repository pour récupérer l'historique
                var repas = await App.ClientRepository.GetRepasAsync(_clientId);

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
