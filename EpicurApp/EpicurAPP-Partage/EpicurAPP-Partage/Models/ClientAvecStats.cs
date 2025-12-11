using EpicurAPP_Partage.Models;

namespace EpicurAppIHM.ViewModels
{
    /// <summary>
    /// ViewModel pour afficher un client avec ses statistiques de visites
    /// </summary>
    public class ClientAvecStats
    {
        /// <summary>
        /// Le client associé
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Informations formatées sur les visites (ex: "5 visite(s) - Dernière : 15/01/2024")
        /// </summary>
        public string InfoVisites { get; set; }
    }
}
