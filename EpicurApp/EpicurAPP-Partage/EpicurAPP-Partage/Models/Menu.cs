namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente un menu composé de plusieurs plats.
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Identifiant unique du menu.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du restaurant auquel appartient ce menu
        /// </summary>
        public int RestaurantId { get; set; }

        /// <summary>
        /// Nom du menu.
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Date de création du menu.
        /// </summary>
        public DateTime DateCreation { get; set; } = DateTime.Now;

        /// <summary>
        /// Statut du menu (Brouillon, Validé, etc.).
        /// </summary>
        public string Statut { get; set; } = "Brouillon";

        /// <summary>
        /// Plat amuse-bouche du menu.
        /// Liste des éléments (plats) du menu.
        /// Permet d'avoir plusieurs plats de la même catégorie.
        /// </summary>
        
        public List<ElementMenu> Elements { get; set; } = new List<ElementMenu>();

        /// <summary>
        /// Note sur 5 du menu.
        /// </summary>
        public int? Note { get; set; }

        /// <summary>
        /// Retours clients sur le menu
        /// </summary>
        public string? Retours { get; set; }

        public DateTime? Date { get; set; }

        public bool EstVerrouille
        {
            get
            {
                bool res = false;
                // Si le service est passé ou s'il reste moins de 48h
                if (!Date.HasValue)
                {
                    res = false;
                }
                else
                {
                    res = (Date.Value - DateTime.Now).TotalHours < 48;
                }
                return res;
            }
        }


        /// <summary>
        /// Indique si le menu est utilisé dans au moins un service.
        /// </summary>
        public bool EstUtilise { get; set; }
    }
}
