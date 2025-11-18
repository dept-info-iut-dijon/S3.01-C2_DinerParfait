using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Table de liaison entre Client et Menu.
    /// Représente l'historique des menus servis aux clients.
    /// </summary>
    public class ClientMenu
    {
        /// <summary>
        /// Identifiant du client.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Identifiant du menu.
        /// </summary>
        public int MenuId { get; set; }
    }
}
