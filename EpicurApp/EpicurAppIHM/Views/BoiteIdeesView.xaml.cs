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
    /// Page d'affichage de la boite a idee
    /// </summary>
    public partial class BoiteIdeesView : UserControl
    {
        /// <summary>
        /// Initialise la liste des idées de plats
        /// </summary>
        public ObservableCollection<IdeePlat> Idees { get; set; } = new ObservableCollection<IdeePlat>();
        /// <summary>
        /// L'instance HttpClient pour les appels API
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// BoiteIdeesView Constructor
        /// </summary>
        public BoiteIdeesView()
        {
            InitializeComponent();
            _httpClient = App.ApiClient.HttpClient;
            DataGridIdees.ItemsSource = Idees;
            ChargerIdees();
        }

        /// <summary>
        /// Charge les idées de plats depuis l'API
        /// </summary>
        /// <exception cref="Exception">Lancée en cas d'erreur lors de l'appel API</exception>
        public async void ChargerIdees()
        {
            try
            {
                List<IdeePlat> idees = await _httpClient.GetFromJsonAsync<List<IdeePlat>>("IdeePlat");
                if (idees != null)
                {
                    Idees.Clear();
                    foreach (IdeePlat idee in idees)
                        Idees.Add(idee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des idées : {ex.Message}",
                                "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ouvre la fiche pour créer une nv idée
        /// </summary>
        private void NouvelleIdee_Click(object sender, RoutedEventArgs e)
        {
            FicheIdee ficheIdee = new FicheIdee();
            bool? result = ficheIdee.ShowDialog();

            if (result == true)
            {
                ChargerIdees();
            }
        }

        /// <summary>
        /// Ouvre la fiche pour modifier une idee
        /// </summary>
        private void DataGridIdees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridIdees.SelectedItem is IdeePlat ideeSelectionnee)
            {
                FicheIdee ficheIdee = new FicheIdee(ideeSelectionnee.Id);
                bool? result = ficheIdee.ShowDialog();

                if (result == true)
                {
                    ChargerIdees();
                }
            }
        }
    }
}
