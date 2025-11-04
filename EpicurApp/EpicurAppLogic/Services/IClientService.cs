using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicurApp_API.Models;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Interface pour la gestion des clients.
    /// Définit les opérations disponibles sur les clients.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Ajoute un nouveau client à la base.
        /// </summary>
        /// <param name="client">Le client à ajouter.</param>
        /// <exception cref="EpicurAppData.Exceptions.InvalidFieldException">
        /// Levée si un champ renseigné est invalide.
        /// </exception>
        void AjouterClient(Client client);
    }
}