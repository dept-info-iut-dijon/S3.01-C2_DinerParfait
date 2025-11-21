using EpicurAPP_Partage.Models;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using System.Text;
using iText.IO.Font;

namespace EpicurAppIHM.Views
{
    /// <summary>
    /// Fenêtre d'affichage de la liste de courses
    /// </summary>
    public partial class AffichageListeCourses : Window
    {
        private List<ElementListeCourse> _listeCourses;

        public AffichageListeCourses(List<ElementListeCourse> listeCourses, string nomMenu)
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // Pour supporter ISO-8859-1
            _listeCourses = listeCourses;
            txtMenuNom.Text = $"Menu : {nomMenu}";
            AfficherListeCourses();
        }

        /// <summary>
        /// Affiche la liste de courses groupée par catégorie
        /// </summary>
        private void AfficherListeCourses()
        {
            ICollectionView listeCoursesView = CollectionViewSource.GetDefaultView(_listeCourses);
            
            // Groupement par catégorie d'ingrédient
            listeCoursesView.GroupDescriptions.Clear();
            listeCoursesView.GroupDescriptions.Add(new PropertyGroupDescription("Ingredient.Categorie", new CategorieIngredientConverter()));
            
            // Tri par catégorie puis par nom
            listeCoursesView.SortDescriptions.Clear();
            listeCoursesView.SortDescriptions.Add(new SortDescription("Ingredient.Categorie", ListSortDirection.Ascending));
            listeCoursesView.SortDescriptions.Add(new SortDescription("Ingredient.Nom", ListSortDirection.Ascending));
            
            ListViewListeCourses.ItemsSource = listeCoursesView;
            
            // Statistiques
            int nombreCategories = _listeCourses.GroupBy(e => e.Ingredient.Categorie).Count();
            txtNombreIngredients.Text = $"{_listeCourses.Count} ingrédients";
            txtNombreCategories.Text = $"{nombreCategories} catégorie(s)";
        }

        /// <summary>
        /// Exporte la liste de courses en PDF
        /// </summary>
        private void Exporter_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Fichiers PDF (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                FileName = $"Liste_Courses_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    // Créer le document PDF
                    using PdfWriter writer = new PdfWriter(saveDialog.FileName);
                    using PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                    // Police d'écriture
                    string cheminPolice = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Police", "arial.ttf");

                    // Vérification
                    if (!System.IO.File.Exists(cheminPolice))
                    {
                        throw new FileNotFoundException($"Le fichier de police est introuvable ici : {cheminPolice}");
                    }

                    PdfFont font = PdfFontFactory.CreateFont(cheminPolice, iText.IO.Font.PdfEncodings.IDENTITY_H);

                    // Titre principal
                    Paragraph titre = new Paragraph("LISTE DE COURSES")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(18);
                    document.Add(titre);
                    
                    // Nom du menu
                    Paragraph nomMenu = new Paragraph(txtMenuNom.Text)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(14)
                        .SetFont(font);
                    document.Add(nomMenu);
                    
                    // Ligne de séparation
                    document.Add(new Paragraph("_".Repeat(50))
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(12));
                    
                    document.Add(new Paragraph("\n"));
                    
                    // Grouper par catégorie
                    IOrderedEnumerable<IGrouping<Ingredient.CategorieIngredient, ElementListeCourse>> parCategorie = _listeCourses.GroupBy(e => e.Ingredient.Categorie)
                                           .OrderBy(g => g.Key);
            
            CategorieIngredientConverter convertisseur = new CategorieIngredientConverter();
            foreach (IGrouping<Ingredient.CategorieIngredient, ElementListeCourse> groupe in parCategorie)
            {
                string nomCategorie = (string)convertisseur.Convert(groupe.Key, null, null, null);
                // Titre de catégorie
                Paragraph titreCategorie = new Paragraph(nomCategorie)
                    .SetFontSize(14)
                    .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                    .SetMarginTop(10);
                document.Add(titreCategorie);
                
                // Liste des ingrédients
                iText.Layout.Element.List liste = new iText.Layout.Element.List();
                foreach (ElementListeCourse element in groupe.OrderBy(e => e.Ingredient.Nom))
                {
                    string texteIngredient = element.Ingredient.Nom;
                    if (element.Quantite > 1)
                        texteIngredient += $" (x{element.Quantite})";

                    ListItem item = new ListItem(texteIngredient);
                    item.SetFont(font);
                    liste.Add(item);
                }
                document.Add(liste);
            }
            
            // Pied de page avec statistiques
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("_".Repeat(50))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
            
            Paragraph stats = new Paragraph($"Total: {_listeCourses.Count} ingrédients")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFont(font);
            document.Add(stats);
            
            Paragraph dateGeneration = new Paragraph($"Générée le {DateTime.Now:dd/MM/yyyy à HH:mm}")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFont(font)
                .SetFontSize(10);
            document.Add(dateGeneration);
            
            document.Close();
            
            MessageBox.Show("Liste de courses exportée en PDF avec succès !", "Succès", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            string debugInfo = $"Message: {ex.Message}\n\nInner: {ex.InnerException?.Message}\n\nStack: {ex.StackTrace}";
            MessageBox.Show(debugInfo, "Debug Info", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

private void Fermer_Click(object sender, RoutedEventArgs e)
{
    Close();
}
    }

    /// <summary>
    /// Convertisseur pour afficher les noms de catégories
    /// </summary>
    public class CategorieIngredientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Ingredient.CategorieIngredient categorie)
            {
                return categorie switch
                {
                    Ingredient.CategorieIngredient.FruitLegume => "Fruits & Légumes",
                    Ingredient.CategorieIngredient.ViandePoisson => "Viande & Poisson",
                    Ingredient.CategorieIngredient.Epicerie => "Épicerie",
                    Ingredient.CategorieIngredient.Cremerie => "Crémerie",
                    Ingredient.CategorieIngredient.Boisson => "Boissons",
                    Ingredient.CategorieIngredient.Autre => "Divers",
                    _ => "Autre"
                };
            }
            return "Autre";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

// Extension method pour la répétition de chaînes
public static class StringExtensions
{
    public static string Repeat(this string input, int count)
    {
        return string.Concat(Enumerable.Repeat(input, count));
    }
}