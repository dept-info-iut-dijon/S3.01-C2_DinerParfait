using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Service des menus
    /// </summary>
    public class MenuService : IMenuService
    {
        private IMenuDAO _menuRepository;

        /// <summary>
        /// Constructeur de la classe MenuService
        /// </summary>
        /// <param name="menuRepository">Le DAO pour interagir avec le menu</param>
        public MenuService(IMenuDAO menuRepository)
        {
            _menuRepository = menuRepository;
        }

        /// <summary>
        /// Ajoute un nouveau menu
        /// </summary>
        /// <param name="menu">menu a ajouter</param>
        /// <exception cref="InvalidFieldException">statut ou nom du menu invalide</exception>
        /// <exception cref="ApplicationException">Impossible d'ajouter le menu</exception>
        public void AjouterMenu(Menu menu)
        {
            if (menu.Statut != "Brouillon" && menu.Statut != "Validé")
            {
                throw new InvalidFieldException("Le statut du menu doit être 'Brouillon' ou 'Validé'.");
            }

            if (string.IsNullOrWhiteSpace(menu.Nom))
            {
                throw new InvalidFieldException("Le nom du menu est obligatoire.");
            }

            try
            {
                _menuRepository.AjouterMenu(menu);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'enregistrement du menu.", ex);
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
                return _menuRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du menu.", ex);
            }
        }

        /// <summary>
        /// Liste tous les menus
        /// </summary>
        /// <returns>Une liste de tout les menus</returns>
        /// <exception cref="ApplicationException">Impossible derecuperer les menus</exception>
        public List<Menu> GetAll()
        {
            try
            {
                return _menuRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des menus.", ex);
            }
        }

        /// <summary>
        /// Ajoute des plats à un menu
        /// </summary>
        /// <param name="menuId">id du menu a completer</param>
        /// <param name="platIds">id des plats a ajouter au menu</param>
        /// <exception cref="InvalidFieldException">Menu ne peut etre vide</exception>
        /// <exception cref="ApplicationException">Impossible d'ajouter les plats au menu</exception>
        public void AjouterPlatsAuMenu(int menuId, List<int> platIds)
        {
            if (platIds == null || platIds.Count == 0)
            {
                throw new InvalidFieldException("Au moins un plat doit être sélectionné pour ajouter au menu.");
            }

            try
            {
                _menuRepository.AjouterPlatsAuMenu(menuId, platIds);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'ajout des plats au menu.", ex);
            }
        }

        /// <summary>
        /// Donne le dernier menu brouillon
        /// </summary>
        /// <returns>Le dernier menu enstatut brouillon</returns>
        /// <exception cref="ApplicationException">Impossible de recuperer le brouillon</exception>
        public Menu? GetDernierBrouillon()
        {
            try
            {
                return _menuRepository.GetDernierBrouillon();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du brouillon de menu.", ex);
            }
        }

        /// <summary>
        /// Met à jour un menu
        /// </summary>
        /// <param name="menu">Menu a mettre a jour</param>
        /// <exception cref="InvalidFieldException">Informations du menuinsuffisantes id,nom</exception>
        /// <exception cref="ApplicationException">Impossible de mettre a jour le menu</exception>
        public void MettreAJourMenu(Menu menu)
        {
            if (menu == null)
            {
                throw new InvalidFieldException("Les informations du menu sont obligatoires.");
            }

            if (menu.Id <= 0)
            {
                throw new InvalidFieldException("L'identifiant du menu est obligatoire pour la mise à jour.");
            }

            if (string.IsNullOrWhiteSpace(menu.Nom))
            {
                throw new InvalidFieldException("Le nom du menu est obligatoire.");
            }

            ValiderStatut(menu.Statut);

            try
            {
                _menuRepository.MettreAJourMenu(menu);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la mise à jour du menu.", ex);
            }
        }

        /// <summary>
        /// Validate le statut du menu
        /// </summary>
        /// <param name="statut">statut du menu</param>
        /// <exception cref="InvalidFieldException">Statut du menu invalide</exception>
        private static void ValiderStatut(string statut)
        {
            if (string.IsNullOrWhiteSpace(statut))
            {
                throw new InvalidFieldException("Le statut du menu est obligatoire.");
            }

            if (!string.Equals(statut, "Brouillon") &&
                !string.Equals(statut, "Validé"))
            {
                throw new InvalidFieldException("Le statut du menu doit être 'Brouillon' ou 'Validé'.");
            }
        }
    }
    
}

