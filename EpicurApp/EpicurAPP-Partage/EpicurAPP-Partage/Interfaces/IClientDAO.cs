using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicurAPP_Partage.Models;

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
        /// Recherche un client par son ID
        /// </summary>
        /// <param name="id">ID du client</param>
        /// <returns>Le client avec l'ID correspodant</returns>
        public Client RechercherClientParId(int id);

        /// <summary>
        /// Récupère l'historique de repas du client
        /// </summary>
        /// <param name="id">Id du client</param>
        Task<Client> GetByIdWithHistoryAsync(int id);
        List<Client> GetAll();
        public Client GetById(int id);

        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);
        void ModifierClient(Client client);

        /// <summary>
        /// Supprime un client de la base de données en fonction de son ID.
        /// </summary>
        /// <param name="id">L'ID du client à supprimer.</param>
        void Delete(int id);
    }

}

