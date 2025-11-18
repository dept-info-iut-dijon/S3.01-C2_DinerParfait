namespace EpicurApp_API.Models
{
    /// <summary>
    /// Énumération des catégories de plats selon les différents services d'un menu gastronomique.
    /// </summary>
    public enum CategoriePlat
    {
        /// <summary>
        /// Amuse-bouche : petite bouchée servie en début de repas
        /// </summary>
        AmuseBouche,

        /// <summary>
        /// Boisson apéritive : boisson servie avant le repas
        /// </summary>
        BoissonAperitif,

        /// <summary>
        /// Entrée : premier plat du repas
        /// </summary>
        Entree,

        /// <summary>
        /// Plat principal : plat central du repas
        /// </summary>
        PlatPrincipal,

        /// <summary>
        /// Vin : boisson servie pendant le repas
        /// </summary>
        Vin,

        /// <summary>
        /// Fromage : plateau de fromages servi avant le dessert
        /// </summary>
        Fromage,

        /// <summary>
        /// Dessert : dernier plat du repas
        /// </summary>
        Dessert
    }
}
