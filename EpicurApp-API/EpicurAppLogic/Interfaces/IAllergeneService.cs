using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les méthodes du service de gestion des allergènes.
    /// </summary>
    /// <remarks>
    /// Cette interface fait le lien entre la couche API (ou logique métier)
    /// et la couche d’accès aux données (DAO) concernant les allergènes.
    /// </remarks>
    public interface IAllergeneService
    {
        /// <summary>
        /// Liste tous les allergènes disponibles.
        /// </summary>
        /// <returns>Liste des allergènes.</returns>
        List<Allergene> GetAll();

        /// <summary>
        /// Liste les allergènes associés à un client spécifique.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des allergènes du client.</returns>
        List<Allergene> GetAllergenesByClient(int clientId);

        /// <summary>
        /// Ajoute une liste d'allergènes à un client spécifique.
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="allergeneIds">Liste des identifiants d'allergènes à ajouter
        /// </summary>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);

        /// <summary>
        /// Ajoute un nouvel allergène à la base de données.
        /// </summary>
        /// <param name="allergene">L'allergène à ajouter.</param>
        void AjouterAllergene(Allergene allergene);
    }
}

