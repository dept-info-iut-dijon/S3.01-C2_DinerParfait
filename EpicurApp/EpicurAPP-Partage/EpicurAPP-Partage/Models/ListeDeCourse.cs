using EpicurAPP_Partage.Models;

namespace EpicurAPP_Partage.ViewModels
{
    public class ListeCourses
    {
        public string NomIngredient { get; set; }
        public string Categorie { get; set; }
        public int Quantite { get; set; } // Nombre d'occurrences dans le menu
        public bool EstAchete { get; set; } // Pour la case à cocher
    }

    public class ListeCoursesViewModel
    {
        public string NomMenu { get; set; }
        // Dictionnaire pour regrouper facilement par catégorie dans la Vue
        public Dictionary<string, List<ListeCourses>> IngredientsParCategorie { get; set; } = new Dictionary<string, List<ListeCourses>>();
    }
}