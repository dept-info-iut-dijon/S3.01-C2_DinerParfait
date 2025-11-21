using Moq;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Services;

public class IdeePlatServiceTests
{
    private readonly Mock<IIdeePlatDAO> _mockIdeePlatDAO;
    private readonly IdeePlatService _ideePlatService;

    public IdeePlatServiceTests()
    {
        _mockIdeePlatDAO = new Mock<IIdeePlatDAO>();
        _ideePlatService = new IdeePlatService(_mockIdeePlatDAO.Object);
    }

    [Fact]
    public void ObtenirToutesLesIdees_RetourneListeIdees()
    {
        List<IdeePlat> ideesAttendues = new List<IdeePlat>
        {
            new IdeePlat { Id = 1, Titre = "Pizza margherita" },
            new IdeePlat { Id = 2, Titre = "Salade César" }
        };
        _mockIdeePlatDAO.Setup(dao => dao.GetAll()).Returns(ideesAttendues);

        List<IdeePlat> resultat = _ideePlatService.ObtenirToutesLesIdees();

        Assert.NotNull(resultat);
        Assert.Equal(2, resultat.Count);
        Assert.Equal(ideesAttendues, resultat);
        _mockIdeePlatDAO.Verify(dao => dao.GetAll(), Times.Once);
    }

    [Fact]
    public void ObtenirToutesLesIdees_DAOLanceException_RetourneApplicationException()
    {
        _mockIdeePlatDAO.Setup(dao => dao.GetAll()).Throws(new Exception("Erreur db"));

        ApplicationException exception = Assert.Throws<ApplicationException>(() =>
            _ideePlatService.ObtenirToutesLesIdees());

        Assert.Equal("Erreur lors de la récupération des idées de plats.", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public void ObtenirIdeeParId_IdValide_RetourneIdee()
    {
        int id = 1;
        IdeePlat ideeAttendue = new IdeePlat { Id = id, Titre = "Poulet rôti" };
        _mockIdeePlatDAO.Setup(dao => dao.GetById(id)).Returns(ideeAttendue);

        IdeePlat? resultat = _ideePlatService.ObtenirIdeeParId(id);

        Assert.NotNull(resultat);
        Assert.Equal(ideeAttendue.Id, resultat.Id);
        Assert.Equal(ideeAttendue.Titre, resultat.Titre);
        _mockIdeePlatDAO.Verify(dao => dao.GetById(id), Times.Once);
    }

    [Fact]
    public void ObtenirIdeeParId_IdInexistant_RetourneNull()
    {
        int id = 99;
        _mockIdeePlatDAO.Setup(dao => dao.GetById(id)).Returns((IdeePlat?)null);

        IdeePlat? resultat = _ideePlatService.ObtenirIdeeParId(id);

        Assert.Null(resultat);
        _mockIdeePlatDAO.Verify(dao => dao.GetById(id), Times.Once);
    }

    [Fact]
    public void ObtenirIdeeParId_DAOLanceException_RetourneApplicationException()
    {
        int id = 1;
        _mockIdeePlatDAO.Setup(dao => dao.GetById(id)).Throws(new Exception("Erreur db"));

        ApplicationException exception = Assert.Throws<ApplicationException>(() =>
            _ideePlatService.ObtenirIdeeParId(id));

        Assert.Equal($"Erreur lors de la récupération de l'idée de plat {id}.", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public void AjouterIdee_IdeeValide_AppelleDAOAjouter()
    {
        IdeePlat ideeValide = new IdeePlat { Titre = "Tacos" };
        _mockIdeePlatDAO.Setup(dao => dao.Ajouter(It.IsAny<IdeePlat>()));

        _ideePlatService.AjouterIdee(ideeValide);

        _mockIdeePlatDAO.Verify(dao => dao.Ajouter(ideeValide), Times.Once);
    }

    [Fact]
    public void AjouterIdee_TitreNull_LanceInvalidFieldException()
    {
        IdeePlat ideeInvalide = new IdeePlat { Titre = null! };

        InvalidFieldException exception = Assert.Throws<InvalidFieldException>(() =>
            _ideePlatService.AjouterIdee(ideeInvalide));

        Assert.Equal("Le titre de l'idée de plat est obligatoire.", exception.Message);
        _mockIdeePlatDAO.Verify(dao => dao.Ajouter(It.IsAny<IdeePlat>()), Times.Never);
    }

    [Fact]
    public void AjouterIdee_TitreVide_LanceInvalidFieldException()
    {
        IdeePlat ideeInvalide = new IdeePlat { Titre = "" };

        InvalidFieldException exception = Assert.Throws<InvalidFieldException>(() =>
            _ideePlatService.AjouterIdee(ideeInvalide));

        Assert.Equal("Le titre de l'idée de plat est obligatoire.", exception.Message);
        _mockIdeePlatDAO.Verify(dao => dao.Ajouter(It.IsAny<IdeePlat>()), Times.Never);
    }

    [Fact]
    public void AjouterIdee_TitreEspaces_LanceInvalidFieldException()
    {
        IdeePlat ideeInvalide = new IdeePlat { Titre = "   " };

        InvalidFieldException exception = Assert.Throws<InvalidFieldException>(() =>
            _ideePlatService.AjouterIdee(ideeInvalide));

        Assert.Equal("Le titre de l'idée de plat est obligatoire.", exception.Message);
        _mockIdeePlatDAO.Verify(dao => dao.Ajouter(It.IsAny<IdeePlat>()), Times.Never);
    }

    [Fact]
    public void AjouterIdee_DAOLanceException_RetourneApplicationException()
    {
        IdeePlat ideeValide = new IdeePlat { Titre = "Burger" };
        _mockIdeePlatDAO.Setup(dao => dao.Ajouter(It.IsAny<IdeePlat>())).Throws(new Exception("Erreur db"));

        ApplicationException exception = Assert.Throws<ApplicationException>(() =>
            _ideePlatService.AjouterIdee(ideeValide));

        Assert.Equal("Erreur lors de l'ajout de l'idée de plat.", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public void ModifierIdee_IdeeValide_AppelleDAOModifier()
    {
        IdeePlat ideeValide = new IdeePlat { Id = 1, Titre = "Sushi" };
        _mockIdeePlatDAO.Setup(dao => dao.Modifier(It.IsAny<IdeePlat>()));

        _ideePlatService.ModifierIdee(ideeValide);

        _mockIdeePlatDAO.Verify(dao => dao.Modifier(ideeValide), Times.Once);
    }

    [Fact]
    public void ModifierIdee_TitreNull_LanceInvalidFieldException()
    {
        IdeePlat ideeInvalide = new IdeePlat { Id = 1, Titre = null! };

        InvalidFieldException exception = Assert.Throws<InvalidFieldException>(() =>
            _ideePlatService.ModifierIdee(ideeInvalide));

        Assert.Equal("Le titre de l'idée de plat est obligatoire.", exception.Message);
        _mockIdeePlatDAO.Verify(dao => dao.Modifier(It.IsAny<IdeePlat>()), Times.Never);
    }

    [Fact]
    public void ModifierIdee_TitreVide_LanceInvalidFieldException()
    {
        IdeePlat ideeInvalide = new IdeePlat { Id = 1, Titre = "" };

        InvalidFieldException exception = Assert.Throws<InvalidFieldException>(() =>
            _ideePlatService.ModifierIdee(ideeInvalide));

        Assert.Equal("Le titre de l'idée de plat est obligatoire.", exception.Message);
        _mockIdeePlatDAO.Verify(dao => dao.Modifier(It.IsAny<IdeePlat>()), Times.Never);
    }

    [Fact]
    public void ModifierIdee_TitreEspaces_LanceInvalidFieldException()
    {
        IdeePlat ideeInvalide = new IdeePlat { Id = 1, Titre = "   " };

        InvalidFieldException exception = Assert.Throws<InvalidFieldException>(() =>
            _ideePlatService.ModifierIdee(ideeInvalide));

        Assert.Equal("Le titre de l'idée de plat est obligatoire.", exception.Message);
        _mockIdeePlatDAO.Verify(dao => dao.Modifier(It.IsAny<IdeePlat>()), Times.Never);
    }

    [Fact]
    public void ModifierIdee_DAOLanceException_RetourneApplicationException()
    {
        IdeePlat ideeValide = new IdeePlat { Id = 1, Titre = "Ramen" };
        _mockIdeePlatDAO.Setup(dao => dao.Modifier(It.IsAny<IdeePlat>())).Throws(new Exception("Erreur db"));

        ApplicationException exception = Assert.Throws<ApplicationException>(() =>
            _ideePlatService.ModifierIdee(ideeValide));

        Assert.Equal("Erreur lors de la modification de l'idée de plat.", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public void SupprimerIdee_IdValide_AppelleDAOSupprimer()
    {
        int id = 1;
        _mockIdeePlatDAO.Setup(dao => dao.Supprimer(id));

        _ideePlatService.SupprimerIdee(id);

        _mockIdeePlatDAO.Verify(dao => dao.Supprimer(id), Times.Once);
    }

    [Fact]
    public void SupprimerIdee_DAOLanceException_RetourneApplicationException()
    {
        int id = 1;
        _mockIdeePlatDAO.Setup(dao => dao.Supprimer(id)).Throws(new Exception("Erreur db"));

        ApplicationException exception = Assert.Throws<ApplicationException>(() =>
            _ideePlatService.SupprimerIdee(id));

        Assert.Equal($"Erreur lors de la suppression de l'idée de plat {id}.", exception.Message);
        Assert.NotNull(exception.InnerException);
    }
}
