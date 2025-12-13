using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        /// Valide le format d'un email.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Pattern regex pour valider un email
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Affiche une erreur sur un champ spécifique.
        /// </summary>
        private void ShowFieldError(TextBlock errorLabel, Border border, string message)
        {
            errorLabel.Text = message;
            errorLabel.Visibility = Visibility.Visible;
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Rouge
        }

        /// <summary>
        /// Réinitialise toutes les erreurs des champs.
        /// </summary>
        private void ClearAllErrors()
        {
            // Masquer le message d'erreur global
            ErrorBorder.Visibility = Visibility.Collapsed;

            // Réinitialiser l'email
            EmailError.Visibility = Visibility.Collapsed;
            EmailBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(212, 175, 55)); // Or

            // Réinitialiser le mot de passe
            PasswordError.Visibility = Visibility.Collapsed;
            PasswordBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(212, 175, 55));

            // Réinitialiser la confirmation
            ConfirmPasswordError.Visibility = Visibility.Collapsed;
            ConfirmPasswordBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(212, 175, 55));

            // Réinitialiser le nom du restaurant
            RestaurantNomError.Visibility = Visibility.Collapsed;
            RestaurantNomBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(212, 175, 55));

            // Réinitialiser la ville
            RestaurantVilleError.Visibility = Visibility.Collapsed;
            RestaurantVilleBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(212, 175, 55));
        }

        /// <summary>
        /// Effectue l'inscription via l'API.
        /// </summary>
        private async Task RegisterAsync()
        {
            // Réinitialiser toutes les erreurs
            ClearAllErrors();

            // Validation des champs
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string restaurantNom = RestaurantNomTextBox.Text.Trim();
            string restaurantVille = RestaurantVilleTextBox.Text.Trim();

            bool hasError = false;

            // Validation de l'email
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowFieldError(EmailError, EmailBorder, "L'email est obligatoire");
                if (!hasError) EmailTextBox.Focus();
                hasError = true;
            }
            else if (!IsValidEmail(email))
            {
                ShowFieldError(EmailError, EmailBorder, "Format d'email invalide (ex: nom@example.com)");
                if (!hasError) EmailTextBox.Focus();
                hasError = true;
            }

            // Validation du mot de passe
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowFieldError(PasswordError, PasswordBorder, "Le mot de passe est obligatoire");
                if (!hasError) PasswordBox.Focus();
                hasError = true;
            }
            else
            {
                // Vérifier la force du mot de passe
                int strength = CalculatePasswordStrength(password);
                if (strength < 4)
                {
                    ShowFieldError(PasswordError, PasswordBorder, "Le mot de passe doit contenir : 8 caractères minimum, une majuscule, une minuscule, un chiffre et un caractère spécial");
                    if (!hasError) PasswordBox.Focus();
                    hasError = true;
                }
            }

            // Validation de la confirmation du mot de passe
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowFieldError(ConfirmPasswordError, ConfirmPasswordBorder, "Veuillez confirmer votre mot de passe");
                if (!hasError) ConfirmPasswordBox.Focus();
                hasError = true;
            }
            else if (password != confirmPassword)
            {
                ShowFieldError(ConfirmPasswordError, ConfirmPasswordBorder, "Les mots de passe ne correspondent pas");
                if (!hasError) ConfirmPasswordBox.Focus();
                hasError = true;
            }

            // Validation du nom du restaurant
            if (string.IsNullOrWhiteSpace(restaurantNom))
            {
                ShowFieldError(RestaurantNomError, RestaurantNomBorder, "Le nom du restaurant est obligatoire");
                if (!hasError) RestaurantNomTextBox.Focus();
                hasError = true;
            }

            // Validation de la ville
            if (string.IsNullOrWhiteSpace(restaurantVille))
            {
                ShowFieldError(RestaurantVilleError, RestaurantVilleBorder, "La ville du restaurant est obligatoire");
                if (!hasError) RestaurantVilleTextBox.Focus();
                hasError = true;
            }

            // Si erreur, arrêter ici
            if (hasError)
            {
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
                        string errorMessage = errorResponse?.Message ?? "Erreur lors de la création du compte";

                        // Gérer les erreurs spécifiques
                        if (errorMessage.Contains("email", StringComparison.OrdinalIgnoreCase) ||
                            errorMessage.Contains("existe déjà", StringComparison.OrdinalIgnoreCase))
                        {
                            ShowFieldError(EmailError, EmailBorder, "Cet email est déjà utilisé");
                            EmailTextBox.Focus();
                        }
                        else if (errorMessage.Contains("mot de passe", StringComparison.OrdinalIgnoreCase))
                        {
                            ShowFieldError(PasswordError, PasswordBorder, errorMessage);
                            PasswordBox.Focus();
                        }
                        else
                        {
                            ShowError(errorMessage);
                        }
                    }
                    catch
                    {
                        ShowError("Erreur lors de la création du compte. Vérifiez vos informations.");
                    }
                }
            }
            catch (HttpRequestException)
            {
                ShowError("❌ Impossible de se connecter au serveur. Vérifiez que l'API est lancée et accessible.");
            }
            catch (Exception ex)
            {
                ShowError($"❌ Une erreur inattendue s'est produite : {ex.Message}");
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
