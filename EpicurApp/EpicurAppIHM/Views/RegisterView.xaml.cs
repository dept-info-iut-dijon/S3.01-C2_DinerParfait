using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Page d'inscription pour créer un nouveau compte utilisateur.
    /// </summary>
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
            EmailTextBox.Focus();
        }

        /// <summary>
        /// Gère le changement du mot de passe pour afficher l'indicateur de force.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordStrength();
            UpdatePasswordMatch();
        }

        /// <summary>
        /// Gère le changement de la confirmation du mot de passe.
        /// </summary>
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordMatch();
        }

        /// <summary>
        /// Met à jour l'indicateur de force du mot de passe.
        /// </summary>
        private void UpdatePasswordStrength()
        {
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(password))
            {
                PasswordStrengthIndicator.Visibility = Visibility.Collapsed;
                return;
            }

            int strength = CalculatePasswordStrength(password);
            PasswordStrengthIndicator.Visibility = Visibility.Visible;

            if (strength >= 4)
            {
                PasswordStrengthIndicator.Text = "✓ Mot de passe fort";
                PasswordStrengthIndicator.Foreground = new SolidColorBrush(Color.FromRgb(34, 197, 94)); // Green
            }
            else if (strength >= 2)
            {
                PasswordStrengthIndicator.Text = "⚠ Mot de passe moyen";
                PasswordStrengthIndicator.Foreground = new SolidColorBrush(Color.FromRgb(251, 146, 60)); // Orange
            }
            else
            {
                PasswordStrengthIndicator.Text = "✗ Mot de passe faible";
                PasswordStrengthIndicator.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
            }
        }

        /// <summary>
        /// Calcule la force du mot de passe (0-5 points).
        /// </summary>
        private int CalculatePasswordStrength(string password)
        {
            int score = 0;
            if (password.Length >= 8) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) score++;
            return score;
        }

        /// <summary>
        /// Met à jour l'indicateur de correspondance des mots de passe.
        /// </summary>
        private void UpdatePasswordMatch()
        {
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(confirmPassword))
            {
                PasswordMatchIndicator.Visibility = Visibility.Collapsed;
                return;
            }

            PasswordMatchIndicator.Visibility = Visibility.Visible;

            if (password == confirmPassword)
            {
                PasswordMatchIndicator.Text = "✓ Les mots de passe correspondent";
                PasswordMatchIndicator.Foreground = new SolidColorBrush(Color.FromRgb(34, 197, 94)); // Green
            }
            else
            {
                PasswordMatchIndicator.Text = "✗ Les mots de passe ne correspondent pas";
                PasswordMatchIndicator.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton d'inscription.
        /// </summary>
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            await RegisterAsync();
        }

        /// <summary>
        /// Gère le clic sur le lien de retour à la connexion.
        /// </summary>
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            new LoginView().Show();
            this.Close();
        }

        /// <summary>
        /// Effectue l'inscription via l'API.
        /// </summary>
        private async Task RegisterAsync()
        {
            // Masquer les messages d'erreur
            ErrorBorder.Visibility = Visibility.Collapsed;

            // Validation des champs
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string restaurantNom = RestaurantNomTextBox.Text.Trim();
            string restaurantVille = RestaurantVilleTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Veuillez entrer votre email");
                EmailTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Veuillez entrer un mot de passe");
                PasswordBox.Focus();
                return;
            }

            // Vérifier la force du mot de passe
            int strength = CalculatePasswordStrength(password);
            if (strength < 4)
            {
                ShowError("Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial");
                PasswordBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("Veuillez confirmer votre mot de passe");
                ConfirmPasswordBox.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Les mots de passe ne correspondent pas");
                ConfirmPasswordBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(restaurantNom))
            {
                ShowError("Veuillez entrer le nom de votre restaurant");
                RestaurantNomTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(restaurantVille))
            {
                ShowError("Veuillez entrer la ville de votre restaurant");
                RestaurantVilleTextBox.Focus();
                return;
            }

            // Désactiver le bouton et afficher le chargement
            RegisterButton.IsEnabled = false;
            LoadingPanel.Visibility = Visibility.Visible;

            try
            {
                // Utiliser le HttpClient partagé
                var client = App.ApiClient.HttpClient;

                // Créer la requête d'inscription
                var registerRequest = new
                {
                    email = email,
                    password = password,
                    confirmPassword = confirmPassword,
                    restaurantNom = restaurantNom,
                    restaurantVille = restaurantVille
                };

                string jsonContent = JsonSerializer.Serialize(registerRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Appel à l'API
                HttpResponseMessage response = await client.PostAsync("/auth/register", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var registerResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, options);

                    if (registerResponse != null && registerResponse.Success)
                    {
                        // Stocker les informations (connexion automatique)
                        App.CurrentUser = registerResponse.Utilisateur;
                        App.CurrentRestaurant = registerResponse.Restaurant;

                        // Configurer le RestaurantId pour toutes les requêtes futures
                        if (registerResponse.Restaurant != null)
                        {
                            App.ApiClient.SetRestaurantId(registerResponse.Restaurant.Id);
                        }

                        // Ouvrir la fenêtre principale
                        new MainWindow().Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError(registerResponse?.Message ?? "Erreur lors de la création du compte");
                    }
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);
                        ShowError(errorResponse?.Message ?? "Erreur lors de la création du compte");
                    }
                    catch
                    {
                        ShowError("Erreur lors de la création du compte");
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
                RegisterButton.IsEnabled = true;
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
}
