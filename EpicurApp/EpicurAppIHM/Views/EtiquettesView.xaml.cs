using EpicurAPP_Partage.Models;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom; // Pour PageSize
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
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
                List<Client> clients = await App.ClientRepository.GetAllAsync();

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
                        
                        // A6 Paysage (148mm x 105mm)
                        PageSize formatPage = PageSize.A6.Rotate();
                        Document document = new Document(pdf, formatPage);
                        document.SetMargins(5, 5, 5, 5);

                        // POLICE
                        string cheminPolice = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Police", "arial.ttf");
                        PdfFont font;
                        if (File.Exists(cheminPolice))
                            font = PdfFontFactory.CreateFont(cheminPolice, PdfEncodings.IDENTITY_H);
                        else
                            font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

                        string dateEvent = datePickerEvenement.SelectedDate?.ToString("dd/MM/yyyy") ?? "";
                        string message = txtMessage.Text;

                        for (int i = 0; i < selection.Count; i++)
                        {
                            InviteSelection invite = selection[i];

                            
                            Paragraph spacer = new Paragraph("")
                                .SetHeight(155f)
                                .SetMargin(0)
                                .SetPadding(0);

                            document.Add(spacer);

                           
                            Table tableContenu = new Table(1); 
                            tableContenu.SetWidth(UnitValue.CreatePercentValue(100)); 
                            tableContenu.SetBorder(iText.Layout.Borders.Border.NO_BORDER); 

                            // Prénom
                            tableContenu.AddCell(new Cell().Add(new Paragraph(invite.Client.Prenom))
                                .SetFont(font)
                                .SetFontSize(14)
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER) 
                                .SetPaddingBottom(0)); 

                            // NOM
                            tableContenu.AddCell(new Cell().Add(new Paragraph(invite.Client.Nom.ToUpper())
                                .SetFont(font)
                                .SetFontSize(22)
                                .SetFontColor(new iText.Kernel.Colors.DeviceRgb(139, 21, 56))
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER))
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER));

                            // Ligne
                            tableContenu.AddCell(new Cell().Add(new Paragraph("__________")
                                .SetFontColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER))
                                .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                .SetPaddingTop(-5) // Remonter un peu la ligne
                                .SetPaddingBottom(5));

                            // Message 
                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                tableContenu.AddCell(new Cell().Add(new Paragraph(message))
                                    .SetFont(font)
                                    .SetFontSize(10)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                            }

                            // Date
                            if (!string.IsNullOrWhiteSpace(dateEvent))
                            {
                                tableContenu.AddCell(new Cell().Add(new Paragraph(dateEvent))
                                    .SetFont(font)
                                    .SetFontSize(8)
                                    .SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY)
                                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                    .SetPaddingTop(5));
                            }

                            document.Add(tableContenu);


                            PliurePdf(pdf, formatPage);

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

        /// <summary>
        /// Genere le trait sur lequel pliser la carte
        /// </summary>
        /// <param name="pdf">pdf sur lequel l'appliquer</param>
        /// <param name="pageSize">taille de ce pdf</param>
        private void PliurePdf(PdfDocument pdf, PageSize pageSize)
        {
            try
            {
                var page = pdf.GetLastPage();
                if (page == null) return;
                var pdfCanvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);

                float middleY = pageSize.GetHeight() / 2;
                float width = pageSize.GetWidth();

                pdfCanvas.SetLineDash(3, 3);
                pdfCanvas.SetLineWidth(0.5f);
                pdfCanvas.SetStrokeColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY);

                pdfCanvas.MoveTo(5, middleY);
                pdfCanvas.LineTo(width - 5, middleY);
                pdfCanvas.Stroke();
            }
            catch { }
        }

        /// <summary>
        /// Ferme la fenêtre
        /// </summary>
        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
