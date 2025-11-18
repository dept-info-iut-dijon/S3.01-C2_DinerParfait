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
        /// </summary>
        List<Allergene> GetAll();

        /// <summary>
        /// </summary>
        List<Allergene> GetAllergenesByClient(int clientId);

        /// <summary>
        /// </summary>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);

        /// <summary>
        /// </summary>
        void AjouterAllergene(Allergene allergene);
    }
}

