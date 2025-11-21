using EpicurAPP_Partage.Models;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom; // Pour PageSize
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using Path = System.IO.Path;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Classe pour représenter un invité avec une sélection
    /// </summary>
    public partial class EtiquettesView : Window
    {
        /// <summary>
        /// L'instance HttpClient pour les appels API
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Liste utilisée pour l'affichage (contient le client + la case à cocher)
        /// </summary>
        public List<InviteSelection> Invites { get; set; } = new List<InviteSelection>();
        /// <summary>
        /// Classe pour représenter un invité avec une sélection
        /// </summary>
        public EtiquettesView()
        {
            InitializeComponent();

            // Configuration iText (Encodage)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _httpClient = App.ApiClient.HttpClient;

            // Date du jour par défaut
            datePickerEvenement.SelectedDate = DateTime.Today;

            ChargerClients();
        }

        /// <summary>
        /// Charge les clients depuis l'API et les lie à la grille
        /// </summary>
        /// <exception cref="Exception">Lance une exception si le chargement échoue</exception>
        private async void ChargerClients()
        {
            try
            {
                // Récupération des clients depuis l'API
                List<Client> clients = await _httpClient.GetFromJsonAsync<List<Client>>("Client");

                if (clients != null)
                {
                    // On transforme chaque Client en InviteSelection pour avoir la case à cocher
                    Invites = clients.Select(c => new InviteSelection { Client = c, EstSelectionne = false }).ToList();

                    // On lie les données à la grille
                    DataGridInvites.ItemsSource = Invites;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Génère le fichier PDF avec les étiquettes pour les invités sélectionnés
        /// </summary>
        private void GenererPDF_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les invités cochés
            List<InviteSelection> selection = Invites.Where(i => i.EstSelectionne).ToList();

            if (selection.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins un invité.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Demander où sauvegarder
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Fichiers PDF (*.pdf)|*.pdf",
                FileName = $"Etiquettes_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                GenererLeFichierPDF(saveDialog.FileName, selection);
            }
        }

        /// <summary>
        /// Génère le fichier PDF avec les étiquettes pour les invités sélectionnés
        /// </summary>
        /// <param name="cheminFichier">Chemin du fichier a savegarder</param>
        /// <param name="selection">Invités sélectionnés</param>
        /// <exception cref="FileNotFoundException">Fiier introuvable</exception>
        private void GenererLeFichierPDF(string cheminFichier, List<InviteSelection> selection)
        {
            try
            {
                PdfWriter writer = new PdfWriter(cheminFichier);
                using (writer)
                {
                    PdfDocument pdf = new PdfDocument(writer);
                    using (pdf)
                    {
                        // CONFIGURATION FORMAT A6
                        Document document = new Document(pdf, PageSize.A6);
                        document.SetMargins(20, 20, 20, 20);

                        //GESTION POLICE
                        string cheminPolice = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Police", "arial.ttf");
                        if (!File.Exists(cheminPolice)) throw new FileNotFoundException("Police introuvable.");
                        PdfFont font = PdfFontFactory.CreateFont(cheminPolice, PdfEncodings.IDENTITY_H);

                        // Récupération des options
                        string dateEvent = datePickerEvenement.SelectedDate?.ToString("dd/MM/yyyy") ?? "";
                        string message = txtMessage.Text;

                        // BOUCLE SUR LES INVITÉS
                        for (int i = 0; i < selection.Count; i++)
                        {
                            InviteSelection invite = selection[i];

                            // Création du contenu de l'étiquette
                            // On ajoute des sauts de ligne pour centrer verticalement
                            document.Add(new Paragraph("\n\n"));

                            // Prénom
                            document.Add(new Paragraph(invite.Client.Prenom)
                                .SetFont(font)
                                .SetFontSize(18)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY));

                            // NOM
                            document.Add(new Paragraph(invite.Client.Nom.ToUpper())
                                .SetFont(font)
                                .SetFontSize(26)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                .SetFontColor(new iText.Kernel.Colors.DeviceRgb(139, 21, 56))); // Couleur Bordeaux #8B1538

                            // Ligne de séparation
                            document.Add(new Paragraph("_________________")
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY));

                            // Message personnalisé
                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                document.Add(new Paragraph(message)
                                    .SetFont(font)
                                    .SetFontSize(12)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                    .SetMarginTop(10));
                            }

                            // Date en bas
                            if (!string.IsNullOrWhiteSpace(dateEvent))
                            {
                                // On positionne la date en bas de page
                                document.Add(new Paragraph($"\n{dateEvent}")
                                    .SetFont(font)
                                    .SetFontSize(10)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                            }

                            //Saut de page (sauf pour le dernier)
                            //AreaBreakType.NEXT_PAGE crée une nouvelle page A6
                            if (i < selection.Count - 1)
                            {
                                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                            }
                        }

                        document.Close();
                    }
                }

                MessageBox.Show("Étiquettes générées avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur PDF : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
