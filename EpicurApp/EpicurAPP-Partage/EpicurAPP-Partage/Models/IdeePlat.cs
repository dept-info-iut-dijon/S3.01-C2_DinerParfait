using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Représente une idée de plat dans la boîte à idées du chef.
    /// </summary>
    public class IdeePlat
    {
        /// <summary>
        /// Identifiant unique de l'idée.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titre de l'idée de plat.
        /// </summary>
        public string Titre { get; set; } = "";

        /// <summary>
        /// Description du plat envisagé.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Catégorie du plat (Entrée, Plat principal, Dessert...).
        /// </summary>
        public string Categorie { get; set; } = "";

        /// <summary>
        /// Notes personnelles du chef.
        /// </summary>
        public string Notes { get; set; } = "";

        /// <summary>
        /// Date de création de l'idée.
        /// </summary>
        public string DateCreation { get; set; } = "";
    }
}