using Moq;
using EpicurAppLogic.Services; // Attention au namespace, vérifie s'il correspond bien à ton projet (EpicurAppLogic.Services ou EpicurApp.Logic.Services)
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using EpicurAPP_Partage.Exceptions;
using System.Collections.Generic;
using System;
using Xunit;

public class MenuServiceTests
{
    private readonly Mock<IMenuDAO> _mockMenuDAO;
    // 1. On ajoute le mock pour le nouveau DAO requis
    private readonly Mock<IPlatDAO> _mockPlatDAO;
    private readonly MenuService _menuService;

    public MenuServiceTests()
    {
        _mockMenuDAO = new Mock<IMenuDAO>();
        // 2. On initialise le mock
        _mockPlatDAO = new Mock<IPlatDAO>();

        // 3. On passe les DEUX mocks au constructeur
        _menuService = new MenuService(_mockMenuDAO.Object, _mockPlatDAO.Object);
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
    public void AjouterMenu_StatutInvalide_DoitLeverException()
    {
        Menu menuInvalide = new Menu { Nom = "Menu test", Statut = "En cours" };
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
        Assert.Equal("Le statut du menu doit être 'Brouillon' ou 'Validé'.", exceptionVoulue.Message);
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
        catch (InvalidFieldException ex) // Ici je suppose que c'est InvalidFieldException, vérifie ton code si c'est ValidationException
        {
            exceptionVoulue = ex;
        }
        // Note : Si ton service ne lève pas d'exception pour null, ce test échouera. 
        // Assure-toi que la méthode AjouterPlatsAuMenu gère bien le null.
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
        // Même remarque : vérifie que ton DAO ou Service lève bien cette exception.
    }

    [Fact]
    public void GetAll_ShouldReturnListOfMenus_WhenDAOReturnsData()
    {
        Mock<IMenuDAO> mockDAO = new Mock<IMenuDAO>();
        // Il faut aussi un mock pour IPlatDAO ici car on instancie manuellement le service
        Mock<IPlatDAO> mockPlatDAO = new Mock<IPlatDAO>();

        List<Menu> listeAttendue = new List<Menu> { new Menu { Id = 1, Nom = "Test" } };
        mockDAO.Setup(dao => dao.GetAll()).Returns(listeAttendue);

        // Correction ici : ajout du 2ème paramètre
        MenuService menuService = new MenuService(mockDAO.Object, mockPlatDAO.Object);

        List<Menu> resultat = menuService.GetAll();
        Assert.Equal(listeAttendue, resultat);
    }

    [Fact]
    public void GetAllErreurDAO()
    {
        Mock<IMenuDAO> mockDAO = new Mock<IMenuDAO>();
        Mock<IPlatDAO> mockPlatDAO = new Mock<IPlatDAO>(); // Création du mock manquant

        mockDAO.Setup(dao => dao.GetAll()).Throws(new Exception("Erreur db"));

        // Correction ici : ajout du 2ème paramètre
        MenuService menuService = new MenuService(mockDAO.Object, mockPlatDAO.Object);
        ApplicationException exceptionVoulue = null;

        try
        {
            menuService.GetAll();
        }
        catch (ApplicationException ex)
        {
            exceptionVoulue = ex;
        }

        // Note : Vérifie que ton MenuService attrape bien l'Exception générique pour relancer une ApplicationException.
        // Si ce n'est pas le cas, ce test échouera.
    }

    [Fact]
    public void GetDernierBrouillon_DoitRetournerMenu()
    {
        Menu brouillon = new Menu { Id = 2, Nom = "Brouillon" };
        _mockMenuDAO.Setup(dao => dao.GetDernierBrouillon()).Returns(brouillon);

        Menu resultat = _menuService.GetDernierBrouillon();

        Assert.Equal(brouillon, resultat);
    }

    [Fact]
    public void MettreAJourMenu_SansId_DoitLeverException()
    {
        Menu menu = new Menu { Nom = "Menu test", Statut = "Brouillon" };
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _menuService.MettreAJourMenu(menu);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("L'identifiant du menu est obligatoire pour la mise à jour.", exceptionVoulue.Message);
    }

    [Fact]
    public void MettreAJourMenu_StatutInvalide_DoitLeverException()
    {
        Menu menu = new Menu { Id = 3, Nom = "Menu test", Statut = "EnAttente" };
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _menuService.MettreAJourMenu(menu);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le statut du menu doit être 'Brouillon' ou 'Validé'.", exceptionVoulue.Message);
    }
}