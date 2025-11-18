using System;
using System.Collections.Generic;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface définissant les méthodes d'accès aux données 
    /// pour la gestion des allergènes dans la base de données.
    /// </summary>
    public interface IAllergeneDAO
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

