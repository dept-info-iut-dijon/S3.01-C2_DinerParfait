using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// idée de plat
    /// </summary>
    public class IdeePlat
    {
        /// <summary>
        /// Id unique
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titre de plat
        /// </summary>
        public string Titre { get; set; } = "";

        /// <summary>
        /// Description du plat
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Categorie du plat
        /// </summary>
        public string Categorie { get; set; } = "";

        /// <summary>
        /// Notes du chef.
        /// </summary>
        public string Notes { get; set; } = "";

        /// <summary>
        /// Date de création
        /// </summary>
        public string DateCreation { get; set; } = "";
    }
}