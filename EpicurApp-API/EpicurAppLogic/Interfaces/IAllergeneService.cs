using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les règles métiers pour la gestion des allergènes
    /// </summary>
    public interface IAllergeneService
    {
        /// <summary>
        /// Récupère tous les allergènes
        /// </summary>
        /// <returns>Liste de tous les allergènes</returns>
        List<Allergene> GetAll();

        /// <summary>
        /// Récupère les allergènes d'un client
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <returns>Liste des allergènes du client</returns>
        List<Allergene> GetAllergenesByClient(int clientId);

        /// <summary>
        /// Associe des allergènes à un client
        /// </summary>
        /// <param name="clientId">Identifiant du client</param>
        /// <param name="allergeneIds">Liste des identifiants des allergènes</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);

        /// <summary>
        /// Ajoute un nouvel allergène
        /// </summary>
        /// <param name="allergene">L'allergène à ajouter</param>
        void AjouterAllergene(Allergene allergene);
    }
}
