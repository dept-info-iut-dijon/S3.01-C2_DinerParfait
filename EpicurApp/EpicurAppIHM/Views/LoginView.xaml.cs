using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Page de connexion pour l'authentification des utilisateurs.
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly HttpClient _httpClient;

        public LoginView()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            EmailTextBox.Focus();
        }

        /// <summary>
        /// Gère le clic sur le bouton de connexion.
        /// </summary>
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await LoginAsync();
        }

        /// <summary>
        /// Permet de se connecter en appuyant sur Entrée dans le champ email.
        /// </summary>
        private async void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        /// <summary>
        /// Permet de se connecter en appuyant sur Entrée dans le champ mot de passe.
        /// </summary>
        private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await LoginAsync();
            }
        }

        /// <summary>
        /// Effectue la connexion à l'API.
        /// </summary>
        private async Task LoginAsync()
        {
            // Masquer les messages d'erreur
            ErrorBorder.Visibility = Visibility.Collapsed;

            // Validation des champs
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Veuillez entrer votre email");
                EmailTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Veuillez entrer votre mot de passe");
                PasswordBox.Focus();
                return;
            }

            // Désactiver le bouton et afficher le chargement
            LoginButton.IsEnabled = false;
            LoadingPanel.Visibility = Visibility.Visible;

            try
            {
                // Utiliser le HttpClient partagé
                var client = App.ApiClient.HttpClient;

                // Créer la requête de connexion
                var loginRequest = new
                {
                    email = email,
                    password = password
                };

                string jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Appel à l'API
                HttpResponseMessage response = await client.PostAsync("/auth/login", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, options);

                    if (loginResponse != null && loginResponse.Success)
                    {
                        // Stocker les informations
                        App.CurrentUser = loginResponse.Utilisateur;
                        App.CurrentRestaurant = loginResponse.Restaurant;

                        // Configurer le RestaurantId pour toutes les requêtes futures
                        if (loginResponse.Restaurant != null)
                        {
                            App.ApiClient.SetRestaurantId(loginResponse.Restaurant.Id);
                        }

                        // Ouvrir la fenêtre principale
                        new MainWindow().Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError(loginResponse?.Message ?? "Erreur de connexion");
                    }
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);
                        ShowError(errorResponse?.Message ?? "Email ou mot de passe invalide");
                    }
                    catch
                    {
                        ShowError("Email ou mot de passe invalide");
                    }
                }
            }
            catch (HttpRequestException)
            {
                ShowError("Impossible de se connecter au serveur. Vérifiez que l'API est lancée.");
            }
            catch (Exception ex)
            {
                ShowError($"Erreur : {ex.Message}");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoadingPanel.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// Affiche un message d'erreur.
        /// </summary>
        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }

    #region Modèles de réponse

    /// <summary>
    /// Modèle pour la réponse de connexion.
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public UtilisateurInfo? Utilisateur { get; set; }
        public RestaurantInfo? Restaurant { get; set; }
    }

    /// <summary>
    /// Informations de l'utilisateur connecté.
    /// </summary>
    public class UtilisateurInfo
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
    }

    /// <summary>
    /// Informations du restaurant.
    /// </summary>
    public class RestaurantInfo
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
    }

    #endregion
}
