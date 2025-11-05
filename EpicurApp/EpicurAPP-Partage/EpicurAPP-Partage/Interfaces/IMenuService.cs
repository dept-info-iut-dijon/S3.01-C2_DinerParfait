using EpicurApp_API.Models;
using System.Collections.Generic;

namespace EpicurAPP_Partage.Interfaces
{
    public interface IMenuService
    {
        void AjouterMenu(Menu menu);
        Menu? GetById(int id);
        List<Menu> GetAll();
        void AjouterPlatsAuMenu(int menuId, List<int> platIds);
    }
}

