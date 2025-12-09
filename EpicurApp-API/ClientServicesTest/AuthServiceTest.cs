using EpicurAppLogic.Interfaces;
using EpicurAppLogic.Services;
using EpicurAPP_Partage.Models;
using Moq;

namespace ClientServicesTest
{
    /// <summary>
    /// Tests unitaires pour AuthService
    /// </summary>
    public class AuthServiceTest
    {
        private readonly Mock<IUtilisateurDAO> _mockUtilisateurDAO;
        private readonly AuthService _authService;

        public AuthServiceTest()
        {
            _mockUtilisateurDAO = new Mock<IUtilisateurDAO>();
            _authService = new AuthService(_mockUtilisateurDAO.Object);
        }


        [Fact]
        public void HashPassword_AvecMotDePasseValide_RetourneHashNonVide()
        {
            string password = "MonMotDePasse123!";

            string hash = _authService.HashPassword(password);

            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void HashPassword_AvecMemeMotDePasse_RetourneMemeHash()
        {
            string password = "MonMotDePasse123!";

            string hash1 = _authService.HashPassword(password);
            string hash2 = _authService.HashPassword(password);

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void HashPassword_AvecMotsDePasseDifferents_RetourneHashDifferents()
        {
            string password1 = "MotDePasse1";
            string password2 = "MotDePasse2";

            string hash1 = _authService.HashPassword(password1);
            string hash2 = _authService.HashPassword(password2);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void HashPassword_AvecMotDePasseVide_RetourneHash()
        {
            string password = "";

            string hash = _authService.HashPassword(password);

            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void HashPassword_AvecCaracteresSpeciaux_RetourneHash()
        {
            string password = "M0t!D€P@ss€#123$%^&*()";

            string hash = _authService.HashPassword(password);

            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void VerifyPassword_AvecMotDePasseCorrect_RetourneTrue()
        {
            string password = "MonMotDePasse123!";
            string hash = _authService.HashPassword(password);

            bool result = _authService.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_AvecMotDePasseIncorrect_RetourneFalse()
        {
            string correctPassword = "MonMotDePasse123!";
            string incorrectPassword = "MauvaisMotDePasse";
            string hash = _authService.HashPassword(correctPassword);

            bool result = _authService.VerifyPassword(incorrectPassword, hash);

            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_AvecHashVide_RetourneFalse()
        {
            string password = "MonMotDePasse123!";
            string emptyHash = "";

            bool result = _authService.VerifyPassword(password, emptyHash);

            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_AvecMotDePasseVideEtHashVide_RetourneTrue()
        {
            string password = "";
            string hash = _authService.HashPassword("");

            bool result = _authService.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_SensibleALaCasse_RetourneFalse()
        {
            string password = "MonMotDePasse";
            string passwordDifferentCasse = "monmotdepasse";
            string hash = _authService.HashPassword(password);

            bool result = _authService.VerifyPassword(passwordDifferentCasse, hash);

            Assert.False(result);
        }


        [Fact]
        public void Login_AvecIdentifiantsValides_RetourneUtilisateur()
        {
            string email = "test@restaurant.com";
            string password = "MotDePasse123!";
            string passwordHash = _authService.HashPassword(password);

            Utilisateur utilisateurAttendu = new Utilisateur
            {
                Id = 1,
                Email = email,
                PasswordHash = passwordHash,
                RestaurantId = 1,
                Restaurant = new Restaurant { Id = 1, Nom = "Restaurant Test" }
            };

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns(utilisateurAttendu);

            Utilisateur? result = _authService.Login(email, password);

            Assert.NotNull(result);
            Assert.Equal(utilisateurAttendu.Id, result.Id);
            Assert.Equal(utilisateurAttendu.Email, result.Email);
            Assert.Equal(utilisateurAttendu.RestaurantId, result.RestaurantId);
        }

        [Fact]
        public void Login_AvecEmailInexistant_RetourneNull()
        {
            string email = "inexistant@restaurant.com";
            string password = "MotDePasse123!";

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns((Utilisateur?)null);

            Utilisateur? result = _authService.Login(email, password);

            Assert.Null(result);
        }

        [Fact]
        public void Login_AvecMotDePasseIncorrect_RetourneNull()
        {
            string email = "test@restaurant.com";
            string correctPassword = "MotDePasse123!";
            string incorrectPassword = "MauvaisMotDePasse";
            string passwordHash = _authService.HashPassword(correctPassword);

            Utilisateur utilisateur = new Utilisateur
            {
                Id = 1,
                Email = email,
                PasswordHash = passwordHash,
                RestaurantId = 1
            };

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns(utilisateur);

            Utilisateur? result = _authService.Login(email, incorrectPassword);

            Assert.Null(result);
        }

        [Fact]
        public void Login_AvecEmailVideEtMotDePasseVide_RetourneNull()
        {
            string email = "";
            string password = "";

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns((Utilisateur?)null);

            Utilisateur? result = _authService.Login(email, password);

            Assert.Null(result);
        }

        [Fact]
        public void Login_AvecEmailValideEtMotDePasseVide_RetourneNull()
        {
            string email = "test@restaurant.com";
            string password = "";
            string passwordHash = _authService.HashPassword("MotDePasseReel");

            Utilisateur utilisateur = new Utilisateur
            {
                Id = 1,
                Email = email,
                PasswordHash = passwordHash,
                RestaurantId = 1
            };

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns(utilisateur);

            Utilisateur? result = _authService.Login(email, password);

            Assert.Null(result);
        }

        [Fact]
        public void Login_EmailSensibleALaCasse_UtiliseEmailExact()
        {
            string emailMajuscule = "TEST@RESTAURANT.COM";
            string emailMinuscule = "test@restaurant.com";
            string password = "MotDePasse123!";
            string passwordHash = _authService.HashPassword(password);

            Utilisateur utilisateur = new Utilisateur
            {
                Id = 1,
                Email = emailMajuscule,
                PasswordHash = passwordHash,
                RestaurantId = 1
            };

            // Le DAO retourne l'utilisateur seulement pour l'email en majuscules
            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(emailMajuscule))
                .Returns(utilisateur);

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(emailMinuscule))
                .Returns((Utilisateur?)null);

            Utilisateur? resultMajuscule = _authService.Login(emailMajuscule, password);
            Utilisateur? resultMinuscule = _authService.Login(emailMinuscule, password);

            Assert.NotNull(resultMajuscule);
            Assert.Null(resultMinuscule);
        }

        [Fact]
        public void Login_AppelleGetByEmailUneFoisAvecBonParametre()
        {
            string email = "test@restaurant.com";
            string password = "MotDePasse123!";

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns((Utilisateur?)null);

            _authService.Login(email, password);

            _mockUtilisateurDAO.Verify(dao => dao.GetByEmail(email), Times.Once);
        }

        [Fact]
        public void Login_AvecUtilisateurMultiplesChamps_RetourneTousLesChamps()
        {
            string email = "admin@restaurant.com";
            string password = "SuperSecretPassword!";
            string passwordHash = _authService.HashPassword(password);

            Restaurant restaurant = new Restaurant
            {
                Id = 5,
                Nom = "Le Grand Restaurant"
            };

            Utilisateur utilisateur = new Utilisateur
            {
                Id = 42,
                Email = email,
                PasswordHash = passwordHash,
                RestaurantId = 5,
                Restaurant = restaurant
            };

            _mockUtilisateurDAO
                .Setup(dao => dao.GetByEmail(email))
                .Returns(utilisateur);

            Utilisateur? result = _authService.Login(email, password);

            Assert.NotNull(result);
            Assert.Equal(42, result.Id);
            Assert.Equal(email, result.Email);
            Assert.Equal(5, result.RestaurantId);
            Assert.NotNull(result.Restaurant);
            Assert.Equal("Le Grand Restaurant", result.Restaurant.Nom);
        }

    }
}
