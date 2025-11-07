using EpicurApp_API.Models;

namespace EpicurAPP_Partage.Interfaces
{
    public interface IPlatService
    {
        /// <summary>
        /// Récupère tous les plats.
        /// </summary>
        /// <returns>Liste de plats</returns>
        List<Plat> ObtenirTousLesPlats();

        /// <summary>
        /// Récupère un plat par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du plat</param>
        /// <returns>Le plat correspondant</returns>
        Plat ObtenirPlatParId(int id);

        /// <summary>
        /// Ajoute un nouveau plat.
        /// </summary>
        /// <param name="plat">Plat à ajouter</param>
        void AjouterPlat(Plat plat);
    }
}

