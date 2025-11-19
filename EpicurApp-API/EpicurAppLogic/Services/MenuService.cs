using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using System.Collections.Generic;
using EpicurAPP_Partage.Exceptions; 
using System.Linq;

namespace EpicurAppLogic.Services
{
    public class MenuService : IMenuService
    {
        private  IMenuDAO _menuDAO;
        private  IPlatDAO _platDAO;

        /// <summary>
        /// constructeur de MenuService.
        /// </summary>
        /// <param name="menuDAO">dao pour le menu</param>
        /// <param name="platDAO">dao pour le plat</param>
        public MenuService(IMenuDAO menuDAO, IPlatDAO platDAO)
        {
            _menuDAO = menuDAO;
            _platDAO = platDAO;
        }

        /// <summary>
        /// Liste de tous les menus.
        /// </summary>
        /// <returns>La liste des menus</returns>
        /// <exception cref="ApplicationException">Erreur de recup des menus</exception>
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
        public Menu? GetById(int id) => _menuDAO.GetById(id);

        /// <summary>
        /// Dernier menu en statut brouillon.
        /// </summary>
        /// <returns>Le dernier menu brouillon</returns>
        public Menu? GetDernierBrouillon() => _menuDAO.GetDernierBrouillon();

        /// <summary>
        /// Ajoute un nouveau menu.
        /// </summary>
        /// <param name="menu">menu a ajouter</param>
        /// <exception cref="InvalidFieldException">nom de menu obligatoire</exception>
        public void AjouterMenu(Menu menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Nom))
                throw new InvalidFieldException("Le nom du menu est obligatoire.");

            ValiderStatut(menu.Statut);

            _menuDAO.AjouterMenu(menu);
        }

        /// <summary>
        /// Met à jour un menu existant.
        /// </summary>
        /// <param name="menu">menu a mettre a jour</param>
        /// <exception cref="InvalidFieldException">Id/nom obligatoire</exception>
        public void MettreAJourMenu(Menu menu)
        {
            if (menu.Id <= 0)
                throw new InvalidFieldException("L'identifiant du menu est obligatoire pour la mise à jour.");

            if (string.IsNullOrWhiteSpace(menu.Nom))
                throw new InvalidFieldException("Le nom du menu est obligatoire.");

            ValiderStatut(menu.Statut);

            _menuDAO.MettreAJourMenu(menu);
        }

        /// <summary>
        /// Ajoute des plats à un menu existant.
        /// </summary>
        /// <param name="menuId">menu a modifier</param>
        /// <param name="platIds">id des plats a ajouter</param>
        /// <exception cref="InvalidFieldException">Minimum un plat par menu</exception>
        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            if (platIds == null || !platIds.Any())
                throw new InvalidFieldException("Au moins un plat doit être sélectionné pour ajouter au menu.");

            _menuDAO.AjouterPlatsAuMenu(menuId, platIds);
        }

        /// <summary>
        /// Génère la liste de courses pour un menu donné.
        /// </summary>
        /// <param name="menuId">Menu dont on et la liste de course</param>
        /// <returns>La liste de course du menu</returns>
        /// <exception cref="InvalidFieldException">Menu non valide</exception>
        public List<ElementListeCourse> GenererListeCourses(int menuId)
        {
            Menu menu = _menuDAO.GetById(menuId);
            if (menu == null) throw new InvalidFieldException($"Menu {menuId} introuvable.");

            List<int> idsPlats = new List<int?>
            {
                menu.AmuseBouche.Id, menu.BoissonAperitif.Id, menu.Entree.Id,
                menu.PlatPrincipal.Id, menu.Vin.Id, menu.Fromage.Id, menu.Dessert.Id
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

            return tousLesIngredients.GroupBy(ing => ing.Id).Select(g => new ElementListeCourse {
                                                                                                    Ingredient = g.First(),
                                                                                                    Quantite = g.Count()
                                                                                                })
                .OrderBy(e => e.Ingredient.Categorie).ThenBy(e => e.Ingredient.Nom).ToList();
        }

        /// <summary>
        /// Valide le statut du menu.
        /// </summary>
        /// <param name="statut">statut du menu</param>
        /// <exception cref="InvalidFieldException">Statut du brouillon incorect</exception>
        private void ValiderStatut(string statut)
        {
            if (statut != "Brouillon" && statut != "Validé")
            {
                throw new InvalidFieldException("Le statut du menu doit être 'Brouillon' ou 'Validé'.");
            }
        }
    }
}