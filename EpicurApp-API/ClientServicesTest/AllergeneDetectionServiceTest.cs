using Moq;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Services;

namespace ClientServicesTest
{
    /// <summary>
    /// Tests unitaires pour AllergeneDetectionService
    /// </summary>
    public class AllergeneDetectionServiceTest
    {
        private readonly Mock<IAllergeneDAO> _mockAllergeneDAO;
        private readonly Mock<IClientDAO> _mockClientDAO;
        private readonly Mock<IMenuDAO> _mockMenuDAO;
        private readonly AllergeneDetectionService _service;

        public AllergeneDetectionServiceTest()
        {
            _mockAllergeneDAO = new Mock<IAllergeneDAO>();
            _mockClientDAO = new Mock<IClientDAO>();
            _mockMenuDAO = new Mock<IMenuDAO>();
            _service = new AllergeneDetectionService(
                _mockAllergeneDAO.Object,
                _mockClientDAO.Object,
                _mockMenuDAO.Object
            );
        }

        #region Tests DetecterConflits

        [Fact]
        public void DetecterConflits_ClientIntrouvable_LanceApplicationException()
        {
            // Arrange
            int clientId = 999;
            int menuId = 1;
            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns((Client?)null);

            // Act & Assert
            ApplicationException exception = Assert.Throws<ApplicationException>(() =>
                _service.DetecterConflits(clientId, menuId));

            Assert.Equal($"Client avec l'ID {clientId} introuvable.", exception.Message);
        }

        [Fact]
        public void DetecterConflits_MenuIntrouvable_LanceApplicationException()
        {
            // Arrange
            int clientId = 1;
            int menuId = 999;
            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns((Menu?)null);

            // Act & Assert
            ApplicationException exception = Assert.Throws<ApplicationException>(() =>
                _service.DetecterConflits(clientId, menuId));

            Assert.Equal($"Menu avec l'ID {menuId} introuvable.", exception.Message);
        }

        [Fact]
        public void DetecterConflits_ClientSansAllergies_RetourneListeVide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(new List<Allergene>());

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflits(clientId, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void DetecterConflits_MenuSansAllergenes_RetourneListeVide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };
            List<Allergene> allergiesClient = new List<Allergene>
            {
                new Allergene { Id = 1, Nom = "Gluten" }
            };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(allergiesClient);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId)).Returns(new List<Allergene>());

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflits(clientId, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void DetecterConflits_PasDeConflits_RetourneListeVide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            List<Allergene> allergiesClient = new List<Allergene>
            {
                new Allergene { Id = 1, Nom = "Gluten" }
            };

            List<Allergene> allergenesMenu = new List<Allergene>
            {
                new Allergene { Id = 2, Nom = "Lactose" }
            };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(allergiesClient);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId)).Returns(allergenesMenu);

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflits(clientId, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void DetecterConflits_AvecConflits_RetourneConflitCorrect()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            Allergene gluten = new Allergene { Id = 1, Nom = "Gluten" };

            List<Allergene> allergiesClient = new List<Allergene> { gluten };
            List<Allergene> allergenesMenu = new List<Allergene> { gluten };

            Ingredient pain = new Ingredient { Id = 1, Nom = "Pain" };
            List<Ingredient> ingredients = new List<Ingredient> { pain };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(allergiesClient);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId)).Returns(allergenesMenu);
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, gluten.Id))
                .Returns(ingredients);

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflits(clientId, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Single(resultat);

            ConflitAllergene conflit = resultat[0];
            Assert.Equal(clientId, conflit.ClientId);
            Assert.Equal("Jean Dupont", conflit.NomClient);
            Assert.Equal(menuId, conflit.MenuId);
            Assert.Equal("Menu du jour", conflit.NomMenu);
            Assert.Single(conflit.AllergenesEnConflit);
            Assert.Equal("Gluten", conflit.AllergenesEnConflit[0].Nom);
            Assert.Single(conflit.IngredientsConcernes);
            Assert.Equal("Pain", conflit.IngredientsConcernes[0].Nom);
            Assert.Equal(NiveauAlerte.Rouge, conflit.Niveau);
            Assert.Contains("Pain", conflit.Message);
            Assert.Contains("Gluten", conflit.Message);
        }

        [Fact]
        public void DetecterConflits_AvecPlusieursAllergenes_RetourneTousLesIngredients()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            Allergene gluten = new Allergene { Id = 1, Nom = "Gluten" };
            Allergene lactose = new Allergene { Id = 2, Nom = "Lactose" };

            List<Allergene> allergiesClient = new List<Allergene> { gluten, lactose };
            List<Allergene> allergenesMenu = new List<Allergene> { gluten, lactose };

            Ingredient pain = new Ingredient { Id = 1, Nom = "Pain" };
            Ingredient lait = new Ingredient { Id = 2, Nom = "Lait" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(allergiesClient);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId)).Returns(allergenesMenu);
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, gluten.Id))
                .Returns(new List<Ingredient> { pain });
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, lactose.Id))
                .Returns(new List<Ingredient> { lait });

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflits(clientId, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Single(resultat);

            ConflitAllergene conflit = resultat[0];
            Assert.Equal(2, conflit.AllergenesEnConflit.Count);
            Assert.Equal(2, conflit.IngredientsConcernes.Count);
        }

        #endregion

        #region Tests DetecterConflitsPourPlusieursClients

        [Fact]
        public void DetecterConflitsPourPlusieursClients_ListeVide_RetourneListeVide()
        {
            // Arrange
            List<int> clientIds = new List<int>();
            int menuId = 1;

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflitsPourPlusieursClients(clientIds, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void DetecterConflitsPourPlusieursClients_AucunConflit_RetourneListeVide()
        {
            // Arrange
            List<int> clientIds = new List<int> { 1, 2 };
            int menuId = 1;

            Client client1 = new Client { Id = 1, Nom = "Dupont", Prenom = "Jean" };
            Client client2 = new Client { Id = 2, Nom = "Martin", Prenom = "Paul" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(1)).Returns(client1);
            _mockClientDAO.Setup(dao => dao.RechercherClientParId(2)).Returns(client2);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(It.IsAny<int>())).Returns(new List<Allergene>());

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflitsPourPlusieursClients(clientIds, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        [Fact]
        public void DetecterConflitsPourPlusieursClients_AvecConflits_RetourneTousLesConflits()
        {
            // Arrange
            List<int> clientIds = new List<int> { 1, 2 };
            int menuId = 1;

            Client client1 = new Client { Id = 1, Nom = "Dupont", Prenom = "Jean" };
            Client client2 = new Client { Id = 2, Nom = "Martin", Prenom = "Paul" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            Allergene gluten = new Allergene { Id = 1, Nom = "Gluten" };
            Ingredient pain = new Ingredient { Id = 1, Nom = "Pain" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(1)).Returns(client1);
            _mockClientDAO.Setup(dao => dao.RechercherClientParId(2)).Returns(client2);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(It.IsAny<int>()))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, gluten.Id))
                .Returns(new List<Ingredient> { pain });

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflitsPourPlusieursClients(clientIds, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(2, resultat.Count);
            Assert.Contains(resultat, c => c.ClientId == 1);
            Assert.Contains(resultat, c => c.ClientId == 2);
        }

        [Fact]
        public void DetecterConflitsPourPlusieursClients_ClientIntrouvable_ContinueAvecLesAutres()
        {
            // Arrange
            List<int> clientIds = new List<int> { 1, 999, 2 };
            int menuId = 1;

            Client client1 = new Client { Id = 1, Nom = "Dupont", Prenom = "Jean" };
            Client client2 = new Client { Id = 2, Nom = "Martin", Prenom = "Paul" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(1)).Returns(client1);
            _mockClientDAO.Setup(dao => dao.RechercherClientParId(999)).Returns((Client?)null);
            _mockClientDAO.Setup(dao => dao.RechercherClientParId(2)).Returns(client2);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(It.IsAny<int>())).Returns(new List<Allergene>());

            // Act
            List<ConflitAllergene> resultat = _service.DetecterConflitsPourPlusieursClients(clientIds, menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat); // Pas de conflits mais ne doit pas planter
        }

        #endregion

        #region Tests ValiderReservation

        [Fact]
        public void ValiderReservation_SansConflits_EstValide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            ReservationRequest request = new ReservationRequest
            {
                ClientId = clientId,
                MenuId = menuId,
                ForceReservation = false
            };

            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId)).Returns(new List<Allergene>());

            // Act
            ValidationReservationResponse resultat = _service.ValiderReservation(request);

            // Assert
            Assert.NotNull(resultat);
            Assert.True(resultat.EstValide);
            Assert.False(resultat.ADesConflits);
            Assert.Equal("Réservation validée. Aucun conflit d'allergène détecté.", resultat.Message);
        }

        [Fact]
        public void ValiderReservation_AvecConflits_SansForce_EstNonValide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            ReservationRequest request = new ReservationRequest
            {
                ClientId = clientId,
                MenuId = menuId,
                ForceReservation = false
            };

            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };
            Allergene gluten = new Allergene { Id = 1, Nom = "Gluten" };
            Ingredient pain = new Ingredient { Id = 1, Nom = "Pain" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, gluten.Id))
                .Returns(new List<Ingredient> { pain });

            // Act
            ValidationReservationResponse resultat = _service.ValiderReservation(request);

            // Assert
            Assert.NotNull(resultat);
            Assert.False(resultat.EstValide);
            Assert.True(resultat.ADesConflits);
            Assert.Single(resultat.Conflits);
            Assert.Equal("Réservation bloquée. Des conflits d'allergènes ont été détectés.", resultat.Message);
        }

        [Fact]
        public void ValiderReservation_AvecConflits_ForceAvecNote_EstValide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            string note = "Le client est informé et accepte le risque";
            ReservationRequest request = new ReservationRequest
            {
                ClientId = clientId,
                MenuId = menuId,
                ForceReservation = true,
                NoteOverride = note
            };

            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };
            Allergene gluten = new Allergene { Id = 1, Nom = "Gluten" };
            Ingredient pain = new Ingredient { Id = 1, Nom = "Pain" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, gluten.Id))
                .Returns(new List<Ingredient> { pain });

            // Act
            ValidationReservationResponse resultat = _service.ValiderReservation(request);

            // Assert
            Assert.NotNull(resultat);
            Assert.True(resultat.EstValide);
            Assert.True(resultat.ADesConflits);
            Assert.True(resultat.EstForcee);
            Assert.Equal(note, resultat.NoteOverride);
            Assert.Contains("Réservation forcée avec note", resultat.Message);
        }

        [Fact]
        public void ValiderReservation_AvecConflits_ForceSansNote_EstNonValide()
        {
            // Arrange
            int clientId = 1;
            int menuId = 1;
            ReservationRequest request = new ReservationRequest
            {
                ClientId = clientId,
                MenuId = menuId,
                ForceReservation = true,
                NoteOverride = ""
            };

            Client client = new Client { Id = clientId, Nom = "Dupont", Prenom = "Jean" };
            Menu menu = new Menu { Id = menuId, Nom = "Menu du jour" };
            Allergene gluten = new Allergene { Id = 1, Nom = "Gluten" };
            Ingredient pain = new Ingredient { Id = 1, Nom = "Pain" };

            _mockClientDAO.Setup(dao => dao.RechercherClientParId(clientId)).Returns(client);
            _mockMenuDAO.Setup(dao => dao.GetById(menuId)).Returns(menu);
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesByClient(clientId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId))
                .Returns(new List<Allergene> { gluten });
            _mockAllergeneDAO.Setup(dao => dao.GetIngredientsByMenuAndAllergene(menuId, gluten.Id))
                .Returns(new List<Ingredient> { pain });

            // Act
            ValidationReservationResponse resultat = _service.ValiderReservation(request);

            // Assert
            Assert.NotNull(resultat);
            Assert.False(resultat.EstValide);
            Assert.True(resultat.ADesConflits);
            Assert.Equal("Une note explicative est requise pour forcer la réservation.", resultat.Message);
        }

        #endregion

        #region Tests GetAllergenesParMenu

        [Fact]
        public void GetAllergenesParMenu_AppelleDAOCorrectement()
        {
            // Arrange
            int menuId = 1;
            List<Allergene> allergenesAttendus = new List<Allergene>
            {
                new Allergene { Id = 1, Nom = "Gluten" },
                new Allergene { Id = 2, Nom = "Lactose" }
            };

            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId)).Returns(allergenesAttendus);

            // Act
            List<Allergene> resultat = _service.GetAllergenesParMenu(menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Equal(2, resultat.Count);
            Assert.Equal("Gluten", resultat[0].Nom);
            Assert.Equal("Lactose", resultat[1].Nom);
            _mockAllergeneDAO.Verify(dao => dao.GetAllergenesParMenu(menuId), Times.Once);
        }

        [Fact]
        public void GetAllergenesParMenu_MenuSansAllergenes_RetourneListeVide()
        {
            // Arrange
            int menuId = 1;
            _mockAllergeneDAO.Setup(dao => dao.GetAllergenesParMenu(menuId)).Returns(new List<Allergene>());

            // Act
            List<Allergene> resultat = _service.GetAllergenesParMenu(menuId);

            // Assert
            Assert.NotNull(resultat);
            Assert.Empty(resultat);
        }

        #endregion
    }
}
