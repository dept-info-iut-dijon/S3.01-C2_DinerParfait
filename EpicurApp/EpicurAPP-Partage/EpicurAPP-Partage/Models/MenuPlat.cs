using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Table de liaison entre Menu et Plat.
    /// Permet d'associer plusieurs plats à un menu (relation many-to-many).
    /// </summary>
    public class MenuPlat
    {
        /// <summary>
        /// Identifiant du menu.
        /// </summary>
        public int MenuId { get; set; }

        /// <summary>
        /// Identifiant du plat.
        /// </summary>
        public int PlatId { get; set; }
    }
}
