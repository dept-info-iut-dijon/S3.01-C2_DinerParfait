namespace EpicurApp_API.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nom {  get; set; }
        public DateTime Date { get; set; }
        public string Statut { get; set; } = "Brouillon";
        public decimal CoutGlobal { get; set; }
        public int TempsPreparationMinutes { get; set; }

        public ICollection<Plat> PlatsAssocies { get; set; }=new List<Plat>();
    }
}
