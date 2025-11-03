using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EpicurAppData;
using EpicurAppLogic.Exceptions;
using EpicurAppLogic.Services;

namespace EpicurAppIHM
{
    /// <summary>
    /// Logique d'interaction pour la fiche client
    /// </summary>
    public partial class FicheClient : Window
    {
        private IClientService _clientService;

        /// <summary>
        /// Constructeur de la fenêtre.
        /// </summary>
        /// <param name="clientService">Service client injecté pour gérer l'enregistrement.</param>
        public FicheClient(IClientService clientService)
        {
            InitializeComponent();
            _clientService = clientService;
        }


        /// <summary>
        /// Enregistre un client en base et affiche un message de confirmation ou d'erreur.
        /// </summary>
        /// <param name="nom">Nom du client.</param>
        /// <param name="prenom">Prénom du client.</param>
        /// <param name="email">Email du client.</param>
        /// <param name="telephone">Téléphone du client.</param>
        /// <param name="telephone">Téléphone du client.</param>
        private void EnregistrerClient(string nom, string prenom, string email, string telephone, string allergies, string note)
        {
            try
            {
                var client = new Client
                {
                    Nom = nom,
                    Prenom = prenom,
                    Email = email,
                    Telephone = telephone,
                    Allergies = allergies,
                    Note = note
                };

                _clientService.AjouterClient(client);
                MessageBox.Show("Client ajouté avec succès !");
            }
            catch (InvalidFieldException ex)
            {
                MessageBox.Show($"Erreur de saisie : {ex.Message}");
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}");
            }
        }
    }
}
