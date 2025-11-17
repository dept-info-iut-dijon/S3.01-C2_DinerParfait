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
    }
}
