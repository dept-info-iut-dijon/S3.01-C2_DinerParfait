using EpicurAPP_Partage.Models;
using System;
using System.Collections.Generic;

namespace EpicurAPP_Partage.Interfaces
{
    /// <summary>
    /// Interface définissant les opérations du service liées à la gestion des clients.
    /// </summary>
    /// <remarks>
    public interface IClientService
    {
        /// <summary>
        /// Ajoute un nouveau client dans la base de données.
        /// </summary>
        /// <param name="client">L’objet <see cref="Client"/> contenant les informations du client à ajouter.</param>
        void AjouterClient(Client client);

        List<Client> ObtenirTousLesClients();
        Client ObtenirClientParId(int id);
        Task<Client> ObtenirClientAvecHistoriqueAsync(int id);
        void AjouterAllergenesAuClient(int id, List<int> allergeneIds);
        void ModifierClient(Client client);
    }
}
