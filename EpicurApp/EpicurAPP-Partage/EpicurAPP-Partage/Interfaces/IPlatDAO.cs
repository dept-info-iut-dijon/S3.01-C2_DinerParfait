using EpicurApp_API.Models;

namespace EpicurAPP_Partage.Interfaces
{
    public interface IPlatDAO
    {
        /// <summary>
        /// Récupère tous les plats de la base.
        /// </summary>
        /// <returns>Liste de plats</returns>
        List<Plat> GetAll();

        /// <summary>
        /// Récupère un plat à partir de son identifiant.
        /// </summary>
        /// <param name="id">Id du plat</param>
        /// <returns>Le plat correspondant ou null si non trouvé</returns>
        Plat? GetById(int id);

        /// <summary>
        /// Ajoute un nouveau plat dans la base.
        /// </summary>
        /// <param name="plat">Plat à ajouter</param>
        void Add(Plat plat);

    
        void Update(Plat plat);

        /// <summary>
        /// Supprime un plat dans la base.
        /// </summary>
        /// <param name="plat">Plat à supprimer</param>
        void Delete(int id);
    }
}
