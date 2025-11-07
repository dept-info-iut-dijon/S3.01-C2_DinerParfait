using System;
using System.Collections.Generic;
using EpicurAPP_Partage.Models;

namespace EpicurAPP_Partage.Interfaces
{
    /// <summary>
    /// Interface définissant les méthodes d'accès aux données 
    /// pour la gestion des allergènes dans la base de données.
    /// </summary>
    public interface IAllergeneDAO
    {
        /// <summary>
        /// Récupère la liste complète des allergènes enregistrés.
        /// </summary>
        /// <returns>Une liste contenant tous les objets <see cref="Allergene"/>.</returns>
        List<Allergene> GetAll();

        /// <summary>
        /// Récupère tous les allergènes associés à un client spécifique.
        /// </summary>
        /// <param name="clientId">Identifiant unique du client.</param>
        /// <returns>Une liste des allergènes liés au client.</returns>
        List<Allergene> GetAllergenesByClient(int clientId);

        /// <summary>
        /// Associe plusieurs allergènes à un client donné.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="allergeneIds">Liste des identifiants d’allergènes à lier.</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);

        /// <summary>
        /// Ajoute un nouvel allergène dans la base de données.
        /// </summary>
        /// <param name="allergene">L’objet <see cref="Allergene"/> à ajouter.</param>
        void AjouterAllergene(Allergene allergene);
    }
}

