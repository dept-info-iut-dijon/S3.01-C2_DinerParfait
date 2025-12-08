using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour la gestion des opérations sur les repas.
    /// </summary>
    public interface IRepasDAO
    {
        /// <summary>
        /// Récupère tous les repas d'un client spécifique avec leurs menus, triés par date décroissante.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des repas du client avec leurs menus associés.</returns>
        List<Repas> GetRepasByClientId(int clientId);

        /// <summary>
        /// Ajoute un nouveau repas dans la base de données.
        /// </summary>
        /// <param name="repas">Le repas à ajouter.</param>
        void AjouterRepas(Repas repas);

        /// <summary>
        /// Récupère un repas par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du repas.</param>
        /// <returns>Le repas trouvé ou null.</returns>
        Repas? GetById(int id);

        /// <summary>
        /// Met à jour les retours d'un repas existant.
        /// </summary>
        /// <param name="repasId">Identifiant du repas.</param>
        /// <param name="retours">Nouveaux retours.</param>
        void ModifierRetours(int repasId, string retours);
    }
}
