namespace EpicurApp_API.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nom {  get; set; }
        public DateTime Date { get; set; }
        public string Statut { get; set; } = "Brouillon";

        // Clés étrangères
        public int? AmuseBoucheId { get; set; }
        public int? BoissonAperitifId { get; set; }
        public int? EntreeId { get; set; }
        public int? PlatPrincipalId { get; set; }
        public int? VinId { get; set; }
        public int? FromageId { get; set; }
        public int? DessertId { get; set; }
        public Plat? AmuseBouche { get; set; }
        public Plat? BoissonAperitif { get; set; }
        public Plat? Entree { get; set; }
        public Plat? PlatPrincipal { get; set; }
        public Plat? Vin { get; set; }
        public Plat? Fromage { get; set; }
        public Plat? Dessert { get; set; }
    }
}
