using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Services;
using Moq;

namespace Test
{
    public class AllergeneServiceTests
    {
        private readonly Mock<IAllergeneDAO> _mockAllergeneDAO;
        private readonly AllergeneService _allergeneService;

        public AllergeneServiceTests()
        {
            _mockAllergeneDAO = new Mock<IAllergeneDAO>();
            _allergeneService = new AllergeneService(_mockAllergeneDAO.Object);
        }

        [Fact]
        public void GetAll_RetourneTousLesAllergenes()
        {
            var allergenesAttendus = new List<Allergene>
        {
            new Allergene { Id = 1, Nom = "Gluten", Description = "Céréales contenant du gluten" },
            new Allergene { Id = 2, Nom = "Lactose", Description = "Produits laitiers" },
            new Allergene { Id = 3, Nom = "Arachides", Description = "Cacahuètes" }
        };

            _mockAllergeneDAO.Setup(dao => dao.GetAll()).Returns(allergenesAttendus);

            var resultat = _allergeneService.GetAll();

            Assert.NotNull(resultat);
            Assert.Equal(3, resultat.Count);
            Assert.Equal("Gluten", resultat[0].Nom);
            Assert.Equal("Lactose", resultat[1].Nom);
            _mockAllergeneDAO.Verify(dao => dao.GetAll(), Times.Once);
        }

        [Fact]
        public void GetAll_RetourneListeVide_QuandAucunAllergene()
        {
            _mockAllergeneDAO.Setup(dao => dao.GetAll()).Returns(new List<Allergene>());
            var resultat = _allergeneService.GetAll();
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void GetAll_LanceApplicationException_QuandErreurDAO()
        {
            Exception exceptionDAO = new Exception("Erreur SQLite");
            _mockAllergeneDAO.Setup(dao => dao.GetAll()).Throws(exceptionDAO);

            ApplicationException exceptionVoulue = null;
            try
            {
                _allergeneService.GetAll();
            }
            catch (ApplicationException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("Erreur lors de la récupération des allergènes.", exceptionVoulue.Message);
            Assert.Equal(exceptionDAO, exceptionVoulue.InnerException);
        }


        [Fact]
        public void GetAllergenesByClient_RetourneAllergenesClient()
        {
            int clientId = 5;
            var allergenesClient = new List<Allergene>
        {
            new Allergene { Id = 1, Nom = "Gluten", Description = "Céréales" },
            new Allergene { Id = 3, Nom = "Arachides", Description = "Cacahuètes" }
        };

            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(allergenesClient);

            var resultat = _allergeneService.GetAllergenesByClient(clientId);

            Assert.NotNull(resultat);
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, a => a.Nom == "Gluten");
            Assert.Contains(resultat, a => a.Nom == "Arachides");
            _mockAllergeneDAO.Verify(dao => dao.GetAllergenesByClient(clientId), Times.Once);
        }

        [Fact]
        public void GetAllergenesByClient_RetourneListeVide_QuandClientSansAllergenes()
        {
            int clientId = 10;
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(new List<Allergene>());

            var resultat = _allergeneService.GetAllergenesByClient(clientId);

            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void GetAllergenesByClient_LanceApplicationException_QuandErreurDAO()
        {
            int clientId = 5;
            Exception exceptionDAO = new Exception("Erreur SQLite");
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Throws(exceptionDAO);

            ApplicationException exceptionVoulue = null;
            try
            {
                _allergeneService.GetAllergenesByClient(clientId);
            }
            catch (ApplicationException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("Erreur lors de la récupération des allergènes du client.", exceptionVoulue.Message);
            Assert.Equal(exceptionDAO, exceptionVoulue.InnerException);
        }

        [Fact]
        public void AjouterAllergenesAuClient_AjouteCorrectement()
        {
            int clientId = 5;
            var allergeneIds = new List<int> { 1, 3, 5 };

            _mockAllergeneDAO.Setup(dao => dao.AjouterAllergenesAuClient(clientId, allergeneIds));

            _allergeneService.AjouterAllergenesAuClient(clientId, allergeneIds);

            _mockAllergeneDAO.Verify(dao => dao.AjouterAllergenesAuClient(clientId, allergeneIds), Times.Once);
        }

        [Fact]
        public void AjouterAllergenesAuClient_LanceInvalidFieldException_QuandListeVide()
        {
            int clientId = 5;
            var allergeneIds = new List<int>();

            InvalidFieldException exceptionVoulue = null;
            try
            {
                _allergeneService.AjouterAllergenesAuClient(clientId, allergeneIds);
            }
            catch (InvalidFieldException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("La liste des allergènes ne peut pas être vide.", exceptionVoulue.Message);
            _mockAllergeneDAO.Verify(dao => dao.AjouterAllergenesAuClient(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public void AjouterAllergenesAuClient_LanceInvalidFieldException_QuandListeNull()
        {
            int clientId = 5;
            List<int> allergeneIds = null;

            InvalidFieldException exceptionVoulue = null;
            try
            {
                _allergeneService.AjouterAllergenesAuClient(clientId, allergeneIds);
            }
            catch (InvalidFieldException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("La liste des allergènes ne peut pas être vide.", exceptionVoulue.Message);
            _mockAllergeneDAO.Verify(dao => dao.AjouterAllergenesAuClient(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public void AjouterAllergenesAuClient_LanceApplicationException_QuandErreurDAO()
        {
            int clientId = 5;
            var allergeneIds = new List<int> { 1, 3 };
            Exception exceptionDAO = new Exception("Erreur SQLite");

            _mockAllergeneDAO.Setup(dao => dao.AjouterAllergenesAuClient(clientId, allergeneIds)).Throws(exceptionDAO);

            ApplicationException exceptionVoulue = null;
            try
            {
                _allergeneService.AjouterAllergenesAuClient(clientId, allergeneIds);
            }
            catch (ApplicationException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("Erreur lors de l'ajout des allergènes au client.", exceptionVoulue.Message);
            Assert.Equal(exceptionDAO, exceptionVoulue.InnerException);
        }

        [Fact]
        public void AjouterAllergene_AjouteCorrectement()
        {
            var allergene = new Allergene { Nom = "Soja", Description = "Produits à base de soja" };

            _mockAllergeneDAO.Setup(dao => dao.AjouterAllergene(allergene));
            _allergeneService.AjouterAllergene(allergene);
            _mockAllergeneDAO.Verify(dao => dao.AjouterAllergene(allergene), Times.Once);
        }

        [Fact]
        public void AjouterAllergene_LanceInvalidFieldException_QuandNomNull()
        {
            var allergeneInvalide = new Allergene { Nom = null, Description = "Description" };

            InvalidFieldException exceptionVoulue = null;
            try
            {
                _allergeneService.AjouterAllergene(allergeneInvalide);
            }
            catch (InvalidFieldException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("Le nom de l'allergène est obligatoire.", exceptionVoulue.Message);
            _mockAllergeneDAO.Verify(dao => dao.AjouterAllergene(It.IsAny<Allergene>()), Times.Never);
        }

        [Fact]
        public void AjouterAllergene_LanceInvalidFieldException_QuandNomVide()
        {
            var allergeneInvalide = new Allergene { Nom = "   ", Description = "Description" };

            InvalidFieldException exceptionVoulue = null;
            try
            {
                _allergeneService.AjouterAllergene(allergeneInvalide);
            }
            catch (InvalidFieldException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("Le nom de l'allergène est obligatoire.", exceptionVoulue.Message);
            _mockAllergeneDAO.Verify(dao => dao.AjouterAllergene(It.IsAny<Allergene>()), Times.Never);
        }

        [Fact]
        public void AjouterAllergene_LanceApplicationException_QuandErreurDAO()
        {
            var allergene = new Allergene { Nom = "Soja", Description = "Produits à base de soja" };
            Exception exceptionDAO = new Exception("Erreur SQLite");

            _mockAllergeneDAO.Setup(dao => dao.AjouterAllergene(allergene)).Throws(exceptionDAO);

            ApplicationException exceptionVoulue = null;
            try
            {
                _allergeneService.AjouterAllergene(allergene);
            }
            catch (ApplicationException ex)
            {
                exceptionVoulue = ex;
            }

            Assert.NotNull(exceptionVoulue);
            Assert.Equal("Erreur lors de l'ajout de l'allergène.", exceptionVoulue.Message);
            Assert.Equal(exceptionDAO, exceptionVoulue.InnerException);
        }
    }
}
