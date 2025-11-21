using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations du service liées à la gestion des idées de plats.
    /// </summary>
    public interface IIdeePlatService
    {
        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        /// <returns>Liste de toutes les idées de plats.</returns>
        List<IdeePlat> ObtenirToutesLesIdees();

        /// <summary>
        /// Récupère une idée de plat par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'idée de plat.</param>
        /// <returns>L'idée de plat correspondante.</returns>
        IdeePlat? ObtenirIdeeParId(int id);

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        /// <param name="idee">L'idée de plat à ajouter.</param>
        void AjouterIdee(IdeePlat idee);

        /// <summary>
        /// Modifie une idée de plat existante.
        /// </summary>
        /// <param name="idee">L'idée de plat avec les données mises à jour.</param>
        void ModifierIdee(IdeePlat idee);

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        /// <param name="id">Identifiant de l'idée de plat à supprimer.</param>
        void SupprimerIdee(int id);
    }
}
