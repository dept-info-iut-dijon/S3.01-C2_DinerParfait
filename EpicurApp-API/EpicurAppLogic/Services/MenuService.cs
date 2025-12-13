using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using EpicurAPP_Partage.Exceptions;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Service des menus
    /// </summary>
    public class MenuService : IMenuService
    {
        /// <summary>
        /// référence au DAO du menu
        /// </summary>
        private readonly IMenuDAO _menuDAO;
        /// <summary>
        /// reférence au DAO des plats
        /// </summary>
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
        /// <exception cref="InvalidOperationException">Le menu ne peut pas être modifié car il est associé à un service dans les 24 heures</exception>
        /// <exception cref="ApplicationException">Impossible de mettre à jour le menu</exception>
        public void MettreAJourMenu(Menu menu)
        {
            if (menu.Id <= 0)
                throw new InvalidFieldException("L'identifiant du menu est obligatoire.");

            if (string.IsNullOrWhiteSpace(menu.Nom))
                throw new InvalidFieldException("Le nom du menu est obligatoire.");

            ValiderStatut(menu.Statut);

            // Vérifier si le menu est associé à un service (passé ou futur)
            if (_menuDAO.EstUtilise(menu.Id))
                throw new InvalidOperationException("Le menu ne peut pas être modifié car il est utilisé dans un service.");

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
            Menu? menu = _menuDAO.GetById(menuId);
            if (menu == null)
                throw new InvalidFieldException($"Menu {menuId} introuvable.");

            // Récupérer tous les IDs de plats depuis les éléments du menu
            List<int> idsPlats = menu.Elements
                .Select(e => e.PlatId)
                .Distinct()
                .ToList();

            List<Ingredient> tousLesIngredients = new List<Ingredient>();

            foreach (int platId in idsPlats)
            {
                Plat? plat = _platDAO.GetById(platId);
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
        /// Récupère le dernier menu en statut brouillon pour un restaurant donné.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Menu en brouillon ou null.</returns>
        public Menu? GetDernierBrouillon(int restaurantId)
        {
            try
            {
                return _menuDAO.GetDernierBrouillon(restaurantId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du dernier brouillon.", ex);
            }
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

        /// <summary>
        /// Supprime un menu par son Id.
        /// </summary>
        /// <param name="id">Id du menu à supprimer</param>
        /// <exception cref="InvalidFieldException">Id invalide</exception>
        /// <exception cref="ApplicationException">Impossible de supprimer le menu</exception>
        public void SupprimerMenu(int id)
        {
            if (id <= 0)
                throw new InvalidFieldException("L'identifiant du menu est invalide.");

            try
            {
                _menuDAO.SupprimerMenu(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la suppression du menu.", ex);
            }
        }

        /// <summary>
        /// Met à jour une note d'un menu.
        /// </summary>
        public void MettreAJourNoteDuMenu(int menuId, int note)
        {
            AjouterNoteAuMenu(menuId, note);
        }

        /// <summary>
        /// Ajoute ou modifie une note à un menu.
        /// </summary>
        public void AjouterNoteAuMenu(int menuId, int note)
        {
            if (note < 0 || note > 5)
            {
                throw new InvalidFieldException("La note doit être comprise entre 0 et 5.");
            }

            Menu? menu = _menuDAO.GetById(menuId);
            if (menu == null)
            {
                throw new EntityNotFoundException("Le menu",menuId);
            }

            menu.Note = note;

            try
            {
                _menuDAO.MettreAJourMenu(menu);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la sauvegarde de la note.", ex);
            }
        }
    }
}