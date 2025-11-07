using Xunit;
using Moq;
using EpicurApp.Logic.Services;       
using EpicurAPP_Partage.Interfaces;  
using EpicurApp_API.Models;          
using EpicurAPP_Partage.Exceptions;  
using System;

public class ClientServiceTests
{
    private readonly Mock<IClientDAO> _mockClientDAO;
    private readonly ClientService _clientService;

    public ClientServiceTests()
    {
        _mockClientDAO = new Mock<IClientDAO>();
        _clientService = new ClientService(_mockClientDAO.Object);
    }


    [Fact]
    public void AjouterClientExeptionField()
    {
        Client clientInvalide = new Client { Nom = null, Prenom = "Marwan" };
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _clientService.AjouterClient(clientInvalide);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }
        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le nom et le prénom sont obligatoires.", exceptionVoulue.Message);
    }

    [Fact]
    public void AjouterClientExeptionVide()
    {
        Client clientInvalide = new Client { Nom = "Himeur", Prenom = "   " };
        InvalidFieldException exceptionVoulue = null;

        try
        {
            _clientService.AjouterClient(clientInvalide);
        }
        catch (InvalidFieldException ex)
        {
            exceptionVoulue = ex;
        }

        Assert.NotNull(exceptionVoulue);
        Assert.Equal("Le nom et le prénom sont obligatoires.", exceptionVoulue.Message);
    }

    [Fact]
    public void AjouterClient_ExeptionDAO()
    {
        Client clientValide = new Client { Nom = "Himeur", Prenom = "Marwan", Email = "mh@ex.fr" };
        Exception exceptionDAO = new Exception("Erreur SQLite");

        // Le mock leve une exeption
        _mockClientDAO.Setup(dao => dao.AjouterClient(clientValide)).Throws(exceptionDAO);

        ApplicationException exceptionVoulue = null;

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
        Assert.Equal(exceptionDAO, exceptionVoulue.InnerException); // On vérifie qu'on a gardé l'exception de base
    }
}