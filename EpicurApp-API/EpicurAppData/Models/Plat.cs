namespace EpicurApp_API.Models
{
    public class Plat
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Categorie { get; set; } //Entrée, Plat, Dessert
        public string IngredientsPrincipaux { get; set; } //Essentiel à l'alerte alergene
        public decimal Cout {  get; set; } //Coût unitaire

        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}
