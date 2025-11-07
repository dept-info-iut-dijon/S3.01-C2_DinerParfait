using Xunit;
using Moq;
using EpicurApp.Logic.Services;
using EpicurAPP_Partage.Interfaces;
using EpicurApp_API.Models;
using EpicurAPP_Partage.Exceptions;
using System;
using System.Collections.Generic;

public class MenuServiceTests
{
    private readonly Mock<IMenuDAO> _mockMenuDAO;
    private readonly MenuService _menuService;

    public MenuServiceTests()
    {
        _mockMenuDAO = new Mock<IMenuDAO>();
        _menuService = new MenuService(_mockMenuDAO.Object);
    }

    [Fact]
    public void AjouterMenuExeptionNom()
    {
        Menu menuInvalide = new Menu { Nom = "   " }; 
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _menuService.AjouterMenu(menuInvalide);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le nom du menu est obligatoire.", exceptionVoulue.Message);
    }

    [Fact]
    public void GetById_APIDAONull()
    {
        _mockMenuDAO.Setup(dao => dao.GetById(99)).Returns((Menu)null);
        Menu resultat = _menuService.GetById(99);
        Assert.Null(resultat);
    }

    [Fact]
    public void AjouterPlatsAuMenuExeptionPlatNull()
    {
        int menuId = 1;
        List<int> platIds = null;
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _menuService.AjouterPlatsAuMenu(menuId, platIds);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Au moins un plat doit être sélectionné pour ajouter au menu.", exceptionVoulue.Message);
    }

    [Fact]
    public void AjouterPlatsAuMenuExeptionPlat()
    {
        int menuId = 1;
        List<int> platIdsVides = new List<int>();
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _menuService.AjouterPlatsAuMenu(menuId, platIdsVides);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Au moins un plat doit être sélectionné pour ajouter au menu.", exceptionVoulue.Message);
    }

    [Fact]
    public void GetAll_ShouldReturnListOfMenus_WhenDAOReturnsData()
    {
        Mock<IMenuDAO> mockDAO = new Mock<IMenuDAO>();
        List<Menu> listeAttendue = new List<Menu> { new Menu { Id = 1, Nom = "Test" } };
        mockDAO.Setup(dao => dao.GetAll()).Returns(listeAttendue);

        MenuService menuService = new MenuService(mockDAO.Object);
        List<Menu> resultat = menuService.GetAll();
        Assert.Equal(listeAttendue, resultat);
    }

    [Fact]
    public void GetAllErreurDAO()
    {
        Mock<IMenuDAO> mockDAO = new Mock<IMenuDAO>();
        mockDAO.Setup(dao => dao.GetAll()).Throws(new Exception("Erreur db"));

        MenuService menuService = new MenuService(mockDAO.Object);
        ApplicationException exceptionVoulue = null;

        try 
        {
            menuService.GetAll(); 
        }
        catch (ApplicationException ex)
        {
            exceptionVoulue = ex; 
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Erreur lors de la récupération des menus.", exceptionVoulue.Message);
    }

}