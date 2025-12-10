using EpicurAPP_Partage.Models;
using System.Windows;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Dialogue d'alerte pour les conflits d'allergènes
    /// </summary>
    public partial class AlerteAllergeneDialog : Window
    {
        /// <summary>
        /// Indique si l'utilisateur a forcé la réservation
        /// </summary>
        public bool ReservationForcee { get; private set; } = false;

        /// <summary>
        /// Note de justification pour l'override
        /// </summary>
        public string? NoteOverride { get; private set; }

        private readonly ConflitAllergene _conflit;

        public AlerteAllergeneDialog(ConflitAllergene conflit)
        {
            InitializeComponent();
            _conflit = conflit;
            AfficherConflit();
        }

        private void AfficherConflit()
        {
            string allergenes = string.Join(", ", _conflit.AllergenesEnConflit);
            txtMessage.Text = $"Attention : Le menu contient des ingrédients allergènes !\n\n" +
                              $"Client : {_conflit.NomClient}\n" +
                              $"Allergènes en conflit : {allergenes}\n\n" +
                              $"Voulez-vous forcer cette réservation malgré le risque ?";
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            ReservationForcee = false;
            DialogResult = false;
            Close();
        }

        private void Forcer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNoteOverride.Text))
            {
                MessageBox.Show("Veuillez saisir une note de justification pour forcer la réservation.",
                    "Note requise", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ReservationForcee = true;
            NoteOverride = txtNoteOverride.Text.Trim();
            DialogResult = true;
            Close();
        }
    }
}