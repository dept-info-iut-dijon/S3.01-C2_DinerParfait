using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour le service de détection des conflits d'allergènes.
    /// </summary>
    public interface IAllergeneDetectionService
    {
        /// <summary>
        /// Détecte les conflits entre les allergies d'un client et les ingrédients d'un menu.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des conflits détectés.</returns>
        List<ConflitAllergene> DetecterConflits(int clientId, int menuId);

        /// <summary>
        /// Détecte les conflits pour plusieurs clients sur un menu.
        /// </summary>
        /// <param name="clientIds">Liste des identifiants des clients.</param>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des conflits détectés pour tous les clients.</returns>
        List<ConflitAllergene> DetecterConflitsPourPlusieursClients(List<int> clientIds, int menuId);

        /// <summary>
        /// Valide une réservation en vérifiant les conflits d'allergènes.
        /// </summary>
        /// <param name="request">Requête de réservation.</param>
        /// <returns>Réponse de validation avec les éventuels conflits.</returns>
        ValidationReservationResponse ValiderReservation(ReservationRequest request);

        /// <summary>
        /// Récupère les allergènes présents dans un menu.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des allergènes présents dans le menu.</returns>
        List<Allergene> GetAllergenesParMenu(int menuId);
    }
}