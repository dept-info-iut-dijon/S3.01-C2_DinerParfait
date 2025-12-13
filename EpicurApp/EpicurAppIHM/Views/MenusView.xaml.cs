using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MenuModel = EpicurAPP_Partage.Models.Menu;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Page d'affichage des menus
    /// </summary>
    public partial class MenusView : UserControl
    {
        /// <summary>
        ///  Collection des menus
        /// </summary>
        public ObservableCollection<MenuModel> Menus { get; set; } = new ObservableCollection<MenuModel>();

        /// <summary>
        /// Intancie la page d'affichage des menus
        /// </summary>
        public MenusView()
        {
            InitializeComponent();
            ListBoxMenus.ItemsSource = Menus;
            ChargerMenus();
        }

        /// <summary>
        /// Charge les menus
        /// </summary>
        /// <exception cref="Exception">Erreur du chargement des menus</exception>
        public async void ChargerMenus()
        {
            try
            {
                // Assurer que le header X-Restaurant-Id est défini
                if (App.CurrentRestaurant != null)
                {
                    App.ApiClient.SetRestaurantId(App.CurrentRestaurant.Id);
                }

                List<MenuModel> menus = await App.MenuRepository.GetAllAsync();

                if (menus != null)
                {
                    Menus.Clear();
                    foreach (MenuModel menu in menus)
                        Menus.Add(menu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des menus : {ex.Message}",
                                "Erreur API", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Consulte le menu sélectionné en double-cliquant dessus
        /// </summary>
        private void ListBoxMenus_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxMenus.SelectedItem is MenuModel menuSelectionne)
            {
                // Toujours ouvrir la fenêtre de consultation (lecture seule)
                // La fenêtre ConsultationMenu aura un bouton "Modifier" pour les brouillons
                ConsultationMenu ficheMenu = new ConsultationMenu(menuSelectionne.Id);
                ficheMenu.Closed += (s, args) => ChargerMenus();
                ficheMenu.ShowDialog();
            }
        }

        /// <summary>
        /// Ouvre la fenêtre de création d'un nouveau menu
        /// </summary>
        private void NouveauMenu_Click(object sender, RoutedEventArgs e)
        {
            CreationMenu creationMenu = new CreationMenu();
            creationMenu.Closed += (s, args) => ChargerMenus(); // Recharger quand la fenêtre se ferme
            creationMenu.ShowDialog();
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