using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicurApp_API.Models;

namespace EpicurAPP_Partage.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des clients.
    /// </summary>
    public interface IClientDAO
    {
        /// <summary>
        /// Ajoute un client dans la base de données.
        /// </summary>
        /// <param name="client">Le client à ajouter.</param>
        void AjouterClient(Client client);
        /// <summary>
        /// Récupère l'historique de repas du client
        /// </summary>
        /// <param name="id">Id du client</param>
        Task<Client> GetByIdWithHistoryAsync(int id);

        /// <summary>
        /// Recherche un client par son ID
        /// </summary>
        /// <param name="id">ID du client</param>
        /// <returns>Le client avec l'ID correspodant</returns>
        public Client rechercherClientParId(int id);
    }
}
