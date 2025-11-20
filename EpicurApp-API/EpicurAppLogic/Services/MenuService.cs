using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using EpicurAPP_Partage.Exceptions; 
using System.Linq;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Service des menus
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly IMenuDAO _menuDAO;
        private readonly IPlatDAO _platDAO;

        /// <summary>
        /// Constructeur de la classe MenuService
        /// </summary>
        /// <param name="menuDAO">Le DAO pour interagir avec le menu</param>
        /// <param name="platDAO">Le DAO pour interagir avec les plats</param>
        public MenuService(IMenuDAO menuDAO, IPlatDAO platDAO)
        {
            _menuDAO = menuDAO;
            _platDAO = platDAO;
        }

        /// <summary>
        /// Liste tous les menus
        /// </summary>
        /// <returns>Une liste de tous les menus</returns>
        /// <exception cref="ApplicationException">Impossible de récupérer les menus</exception>
        public List<Menu> GetAll()
        {
            try
            {
                return _menuDAO.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des menus.", ex);
            }
        }

        /// <summary>
        /// Donne un menu par son id
        /// </summary>
        /// <param name="id">id du menu cherché</param>
        /// <returns>le menu avec l'id correspondant</returns>
        /// <exception cref="ApplicationException">Impossible de trouver le menu</exception>
        public Menu? GetById(int id)
        {
            try
            {
                return _menuDAO.GetById(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du menu.", ex);
            }
        }

        /// <summary>
        /// Donne le dernier menu brouillon
        /// </summary>
        /// <returns>Le dernier menu en statut brouillon</returns>
        /// <exception cref="ApplicationException">Impossible de récupérer le brouillon</exception>
        public Menu? GetDernierBrouillon()
        {
            try
            {
                return _menuDAO.GetDernierBrouillon();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du brouillon de menu.", ex);
            }
        }

        /// <summary>
        /// Ajoute un nouveau menu
        /// </summary>
        /// <param name="menu">menu a ajouter</param>
        /// <exception cref="InvalidFieldException">nom de menu obligatoire</exception>
        /// <exception cref="ApplicationException">Impossible d'ajouter le menu</exception>
        public void AjouterMenu(Menu menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Nom))
                throw new InvalidFieldException("Le nom du menu est obligatoire.");

            ValiderStatut(menu.Statut);

            try
            {
                _menuDAO.AjouterMenu(menu);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'enregistrement du menu.", ex);
            }
        }

        /// <summary>
        /// Met à jour un menu
        /// </summary>
        /// <param name="menu">Menu a mettre à jour</param>
        /// <exception cref="InvalidFieldException">Informations du menu insuffisantes id,nom</exception>
        /// <exception cref="ApplicationException">Impossible de mettre à jour le menu</exception>
        public void MettreAJourMenu(Menu menu)
        {
            if (menu.Id <= 0)
                throw new InvalidFieldException("L'identifiant du menu est obligatoire pour la mise à jour.");

            if (string.IsNullOrWhiteSpace(menu.Nom))
                throw new InvalidFieldException("Le nom du menu est obligatoire.");

            ValiderStatut(menu.Statut);

            try
            {
                _menuDAO.MettreAJourMenu(menu);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la mise à jour du menu.", ex);
            }
        }

        /// <summary>
        /// Ajoute des plats à un menu existant.
        /// </summary>
        /// <param name="menuId">menu à modifier</param>
        /// <param name="platIds">id des plats à ajouter</param>
        /// <exception cref="InvalidFieldException">Minimum un plat par menu</exception>
        /// <exception cref="ApplicationException">Impossible d'ajouter les plats au menu</exception>
        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            if (platIds == null || !platIds.Any())
                throw new InvalidFieldException("Au moins un plat doit être sélectionné pour ajouter au menu.");

            try
            {
                _menuDAO.AjouterPlatsAuMenu(menuId, platIds);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'ajout des plats au menu.", ex);
            }
        }

        /// <summary>
        /// Génère la liste de courses pour un menu donné.
        /// </summary>
        /// <param name="menuId">Menu dont on veut la liste de course</param>
        /// <returns>La liste de course du menu</returns>
        /// <exception cref="InvalidFieldException">Menu non valide</exception>
        public List<ElementListeCourse> GenererListeCourses(int menuId)
        {
            Menu menu = _menuDAO.GetById(menuId);
            if (menu == null) 
                throw new InvalidFieldException($"Menu {menuId} introuvable.");

            List<int> idsPlats = new List<int?>
            {
                menu.AmuseBouche?.Id, 
                menu.BoissonAperitif?.Id, 
                menu.Entree?.Id,
                menu.PlatPrincipal?.Id, 
                menu.Vin?.Id, 
                menu.Fromage?.Id, 
                menu.Dessert?.Id
            }.Where(id => id.HasValue).Select(id => id.Value).ToList();

            List<Ingredient> tousLesIngredients = new List<Ingredient>();

            foreach (int platId in idsPlats)
            {
                Plat plat = _platDAO.GetById(platId);
                if (plat?.IngredientsPrincipaux != null)
                {
                    tousLesIngredients.AddRange(plat.IngredientsPrincipaux);
                }
            }

            return tousLesIngredients
                .GroupBy(ing => ing.Id)
                .Select(g => new ElementListeCourse 
                {
                    Ingredient = g.First(),
                    Quantite = g.Count()
                })
                .OrderBy(e => e.Ingredient.Categorie)
                .ThenBy(e => e.Ingredient.Nom)
                .ToList();
        }

        /// <summary>
        /// Valide le statut du menu.
        /// </summary>
        /// <param name="statut">statut du menu</param>
        /// <exception cref="InvalidFieldException">Statut du menu incorrect</exception>
        private void ValiderStatut(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
            {
                throw new InvalidFieldException("Le statut du menu est obligatoire.");
            }

            if (statut != "Brouillon" && statut != "Validé")
            {
                throw new InvalidFieldException("Le statut du menu doit être 'Brouillon' ou 'Validé'.");
            }
        }
    }
}