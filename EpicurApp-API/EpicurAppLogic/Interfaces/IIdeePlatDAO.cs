using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des idées de plats.
    /// </summary>
    public interface IIdeePlatDAO
    {
        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        /// <returns>Liste de toutes les idées de plats.</returns>
        List<IdeePlat> GetAll();

        /// <summary>
        /// Récupère une idée de plat par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'idée de plat.</param>
        /// <returns>L'idée de plat trouvée ou null.</returns>
        IdeePlat? GetById(int id);

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        /// <param name="idee">L'idée de plat à ajouter.</param>
        void Ajouter(IdeePlat idee);

        /// <summary>
        /// Modifie une idée de plat existante.
        /// </summary>
        /// <param name="idee">L'idée de plat avec les données mises à jour.</param>
        void Modifier(IdeePlat idee);

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        /// <param name="id">Identifiant de l'idée de plat à supprimer.</param>
        void Supprimer(int id);
    }
}
