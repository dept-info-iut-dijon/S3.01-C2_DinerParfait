using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurApp_API.Models;

namespace EpicurApp.Logic.Services
{
    public class MenuService : IMenuService
    {
        private IMenuDAO _menuRepository;

        public MenuService(IMenuDAO menuRepository)
        {
            _menuRepository = menuRepository;
        }

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

