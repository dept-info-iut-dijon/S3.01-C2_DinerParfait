using System.Windows;
using EpicurApp_API.Models;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;

namespace EpicurAppIHM
{
    /// <summary>
    /// Logique d'interaction pour la fiche client
    /// </summary>
    public partial class FicheClient : Window
    {
        private IClientService _clientService;

        public FicheClient(IClientService clientService)
        {
            InitializeComponent();
            _clientService = clientService;
        }

        // Vide tous les champs
        private void Annuler(object sender, RoutedEventArgs e)
        {
            txtPrenom.Clear();
            txtNom.Clear();
            txtEmail.Clear();
            txtTelephone.Clear();
            txtCarte.Clear();
            txtAllergies.Clear();
            txtPlats.Clear();
        }

        // Crée le client en base via le service
        private void CreerClient(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new Client
                {
                    Nom = txtNom.Text,
                    Prenom = txtPrenom.Text,
                    Email = txtEmail.Text,
                    Telephone = txtTelephone.Text,
                    Allergies = txtAllergies.Text,
                    Notes = txtPlats.Text
                };

                _clientService.AjouterClient(client);

                MessageBox.Show("Client ajouté avec succès !",
                                "Succès",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                Annuler(sender, e);
            }
            catch (InvalidFieldException ex)
            {
                MessageBox.Show($"Erreur de saisie : {ex.Message}",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}

