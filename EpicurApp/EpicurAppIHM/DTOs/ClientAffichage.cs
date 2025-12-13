using EpicurAPP_Partage.Models;

namespace EpicurAppIHM.DTOs
{
    /// <summary>
    /// DTO pour afficher un client avec son statut inactif et VIP
    /// </summary>
    public class ClientAffichage
    {
        /// <summary>
        /// Le client associé
        /// </summary>
        public Client Client { get; set; } = new Client();

        /// <summary>
        /// Indique si le client est inactif (60+ jours sans visite)
        /// </summary>
        public bool EstInactif { get; set; }

        /// <summary>
        /// Indique si le client est VIP (3+ visites)
        /// </summary>
        public bool EstVIP { get; set; }

        /// <summary>
        /// Icône de statut : "*" pour VIP, "!" pour inactif, "" sinon
        /// </summary>
        public string IconeStatut => EstVIP ? "*" : (EstInactif ? "!" : "");
    }
}
