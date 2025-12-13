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
        /// Liste tous les allergènes disponibles.
        /// </summary>
        /// <returns>La liste des allergene</returns>
        List<Allergene> GetAll();

        /// <summary>
        /// Liste les allergènes associés à un client spécifique.
        /// </summary>
        /// <param name="clientId">id du client dont on veut les allergene</param>
        /// <returns>La liste d'allergie du client</returns>
        List<Allergene> GetAllergenesByClient(int clientId);

        /// <summary>
        /// Ajoute une liste d'allergènes à un client spécifique.
        /// </summary>
        /// <param name="clientId">id du client a qui on veut ajouter des allergene</param>
        /// <param name="allergeneIds">id des alergene a ajouter</param>
        void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds);

        /// <summary>
        /// Méthode pour ajouter un nouvel allergène à la base de données.
        /// </summary>
        /// <param name="allergene">allergene a ajouter</param>
        void AjouterAllergene(Allergene allergene);

        /// <summary>
        /// Récupère les allergènes présents dans les ingrédients d'un menu.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des allergènes présents dans le menu.</returns>
        List<Allergene> GetAllergenesParMenu(int menuId);

        /// <summary>
        /// Récupère les ingrédients d'un menu qui contiennent un allergène spécifique.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <param name="allergeneId">Identifiant de l'allergène.</param>
        /// <returns>Liste des ingrédients concernés.</returns>
        List<Ingredient> GetIngredientsByMenuAndAllergene(int menuId, int allergeneId);
    }
}

