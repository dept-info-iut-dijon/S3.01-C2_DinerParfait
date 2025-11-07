using Xunit;
using Moq;
using EpicurApp.Logic.Services;
using EpicurAPP_Partage.Interfaces;
using EpicurApp_API.Models;
using EpicurAPP_Partage.Exceptions;
using System;
using System.Collections.Generic;

public class PlatServiceTests
{
    private readonly Mock<IPlatDAO> _mockPlatDAO;
    private readonly PlatService _platService;

    public PlatServiceTests()
    {
        _mockPlatDAO = new Mock<IPlatDAO>();
        _platService = new PlatService(_mockPlatDAO.Object);
    }

    [Fact]
    public void ObtenirTousLesPlats()
    {
        List<Plat> platsAttendus = new List<Plat>
        {
            new Plat { Id = 1, Nom = "veau" },
            new Plat { Id = 2, Nom = "agneau" }
        };
        _mockPlatDAO.Setup(dao => dao.GetAll()).Returns(platsAttendus);

        List<Plat> resultat = _platService.ObtenirTousLesPlats();

        Assert.NotNull(resultat);
        Assert.Equal(2, resultat.Count);
        Assert.Equal(platsAttendus, resultat);
    }

    [Fact]
    public void ObtenirPlatParIdErreurPasTrouve()
    {
        int id = 99;
        // DAO renvoei rien
        _mockPlatDAO.Setup(dao => dao.GetById(id)).Returns((Plat)null);

        ApplicationException exceptionVoulue = null;

        try
        {
            _platService.ObtenirPlatParId(id);
        }
        catch (ApplicationException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Erreur lors de la récupération du plat.", exceptionVoulue.Message);
        Assert.Equal($"Le plat avec l'id {id} n'existe pas.", exceptionVoulue.InnerException.Message);
    }

    [Fact]
    public void AjouterPlatExeptionNom()
    {
        Plat platInvalide = new Plat { Nom = null, Categorie = "Dessert" };
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _platService.AjouterPlat(platInvalide);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le nom du plat est obligatoire.", exceptionVoulue.Message);
        _mockPlatDAO.Verify(dao => dao.Add(It.IsAny<Plat>()), Times.Never);
    }

    [Fact]
    public void ObtenirPlatParIdExeptionDAO()
    {
        Mock<IPlatDAO> mockDAO = new Mock<IPlatDAO>();
        mockDAO.Setup(dao => dao.GetById(1)).Throws(new Exception("Erreur db"));

        PlatService platService = new PlatService(mockDAO.Object);
        ApplicationException exceptionVoulue = null;

        try { platService.ObtenirPlatParId(1); }
        catch (ApplicationException ex) { exceptionVoulue = ex; }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Erreur lors de la récupération du plat.", exceptionVoulue.Message);
    }
}