using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Classe représentant un allergène
    /// </summary>
    public class Allergene
    {
        /// <summary>
        /// Identifiant de l'allergène
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nom de l'allergène
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        /// Description de l'allergène
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Affichage de l'allergène
        /// </summary>
        /// <returns>Representation de l'allergene</returns>
        public override string ToString()
        {
            return Nom;
        }
    }
}
