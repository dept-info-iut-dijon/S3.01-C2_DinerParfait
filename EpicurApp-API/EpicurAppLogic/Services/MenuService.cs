using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
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
    }
}

