using Moq;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Services;

public class ClientServiceTests
{
    private readonly Mock<IClientDAO> _mockClientDAO;
    private readonly Mock<IRepasDAO> _mockRepasDAO;
    private readonly ClientService _clientService;

    public ClientServiceTests()
    {
        _mockClientDAO = new Mock<IClientDAO>();
        _mockRepasDAO = new Mock<IRepasDAO>();
        _clientService = new ClientService(_mockClientDAO.Object, _mockRepasDAO.Object);
    }


    [Fact]
    public void AjouterClientExeptionField()
    {
        Client clientInvalide = new Client { Nom = null!, Prenom = "Marwan" };
        InvalidFieldException? exceptionVoulue = null;

        try
        {
            _clientService.AjouterClient(clientInvalide);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }
        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le nom et le prenom sont obligatoires.", exceptionVoulue.Message);
    }

    [Fact]
    public void AjouterClientExeptionVide()
    {
        Client clientInvalide = new Client { Nom = "Himeur", Prenom = "   " };
        InvalidFieldException? exceptionVoulue = null;

        try
        {
            _clientService.AjouterClient(clientInvalide);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le nom et le prenom sont obligatoires.", exceptionVoulue.Message);
    }

    [Fact]
    public void AjouterClient_ExeptionDAO()
    {
        Client clientValide = new Client { Nom = "Himeur", Prenom = "Marwan", Email = "mh@ex.fr" };
        Exception exceptionDAO = new Exception("Erreur SQLite");

        // Le mock leve une exeption
        _mockClientDAO.Setup(dao => dao.AjouterClient(clientValide)).Throws(exceptionDAO);

        ApplicationException? exceptionVoulue = null;

        try
        {
            _clientService.AjouterClient(clientValide);
        }
        catch (ApplicationException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Erreur lors de l'enregistrement du client.", exceptionVoulue.Message);
        Assert.Equal(exceptionDAO, exceptionVoulue.InnerException); // On v�rifie qu'on a gard� l'exception de base
    }
}