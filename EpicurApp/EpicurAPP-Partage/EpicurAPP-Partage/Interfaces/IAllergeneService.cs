using System;
using System.Collections.Generic;
using EpicurAPP_Partage.Models;

namespace EpicurAPP_Partage.Interfaces
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
        /// Récupère la liste complète des allergènes disponibles.
        /// </summary>
        /// <returns>Une liste contenant tous les objets <see cref="Allergene"/>.</returns>
        List<Allergene> GetAll();

        /// <summary>
        /// Récupère tous les allergènes associés à un client spécifique.
        /// </summary>
        /// <param name="clientId">Identifiant unique du client.</param>
        /// <returns>Une liste d’allergènes liés à ce client.</returns>
        List<Allergene> GetAllergenesByClient(int clientId);

        /// <summary>
        /// Ajoute plusieurs allergènes à un client donné.
        /// </summary>
        /// <param name="clientId">Identifiant du client concerné.</param>
        /// <param name="allergeneIds">Liste des identifiants d’allergènes à associer.</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);

        /// <summary>
        /// Ajoute un nouvel allergène dans la base de données.
        /// </summary>
        /// <param name="allergene">L’objet <see cref="Allergene"/> à insérer.</param>
        void AjouterAllergene(Allergene allergene);
    }
}

