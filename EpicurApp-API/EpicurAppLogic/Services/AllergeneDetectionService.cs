using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Service de détection des conflits d'allergènes entre clients et menus.
    /// </summary>
    public class AllergeneDetectionService : IAllergeneDetectionService
    {
        private readonly IAllergeneDAO _allergeneDAO;
        private readonly IClientDAO _clientDAO;
        private readonly IMenuDAO _menuDAO;

        /// <summary>
        /// Constructeur du service de détection d'allergènes.
        /// </summary>
        /// <param name="allergeneDAO">DAO des allergènes.</param>
        /// <param name="clientDAO">DAO des clients.</param>
        /// <param name="menuDAO">DAO des menus.</param>
        public AllergeneDetectionService(
            IAllergeneDAO allergeneDAO, 
            IClientDAO clientDAO, 
            IMenuDAO menuDAO)
        {
            _allergeneDAO = allergeneDAO;
            _clientDAO = clientDAO;
            _menuDAO = menuDAO;
        }

        /// <summary>
        /// Détecte les conflits entre les allergies d'un client et les ingrédients d'un menu.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des conflits détectés.</returns>
        public List<ConflitAllergene> DetecterConflits(int clientId, int menuId)
        {
            List<ConflitAllergene> conflits = new List<ConflitAllergene>();

            // Récupérer les informations du client
            Client? client = _clientDAO.RechercherClientParId(clientId);
            if (client == null)
            {
                throw new ApplicationException($"Client avec l'ID {clientId} introuvable.");
            }

            // Récupérer le menu
            Menu? menu = _menuDAO.GetById(menuId);
            if (menu == null)
            {
                throw new ApplicationException($"Menu avec l'ID {menuId} introuvable.");
            }

            // Récupérer les allergies du client
            List<Allergene> allergiesClient = _allergeneDAO.GetAllergenesByClient(clientId);
            if (allergiesClient.Count == 0)
            {
                // Pas d'allergies, pas de conflits
                return conflits;
            }

            // Récupérer les allergènes présents dans le menu
            List<Allergene> allergenesMenu = _allergeneDAO.GetAllergenesParMenu(menuId);
            if (allergenesMenu.Count == 0)
            {
                // Pas d'allergènes dans le menu, pas de conflits
                return conflits;
            }

            // Trouver les allergènes en commun
            List<Allergene> allergenesEnConflit = allergiesClient
                .Where(ac => allergenesMenu.Any(am => am.Id == ac.Id))
                .ToList();

            if (allergenesEnConflit.Count > 0)
            {
                // Récupérer tous les ingrédients concernés
                List<Ingredient> ingredientsConcernes = new List<Ingredient>();
                foreach (var allergene in allergenesEnConflit)
                {
                    var ingredients = _allergeneDAO.GetIngredientsByMenuAndAllergene(menuId, allergene.Id);
                    ingredientsConcernes.AddRange(ingredients);
                }
                // Supprimer les doublons
                ingredientsConcernes = ingredientsConcernes
                    .GroupBy(i => i.Id)
                    .Select(g => g.First())
                    .ToList();

                // Créer le conflit
                ConflitAllergene conflit = new ConflitAllergene
                {
                    ClientId = clientId,
                    NomClient = $"{client.Prenom} {client.Nom}",
                    MenuId = menuId,
                    NomMenu = menu.Nom,
                    AllergenesEnConflit = allergenesEnConflit,
                    IngredientsConcernes = ingredientsConcernes,
                    Niveau = NiveauAlerte.Rouge, // Par défaut, alerte bloquante
                    Message = GenererMessageAlerte(client, allergenesEnConflit, ingredientsConcernes)
                };

                conflits.Add(conflit);
            }

            return conflits;
        }

        /// <summary>
        /// Détecte les conflits pour plusieurs clients sur un menu.
        /// </summary>
        /// <param name="clientIds">Liste des identifiants des clients.</param>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des conflits détectés pour tous les clients.</returns>
        public List<ConflitAllergene> DetecterConflitsPourPlusieursClients(List<int> clientIds, int menuId)
        {
            List<ConflitAllergene> tousLesConflits = new List<ConflitAllergene>();

            foreach (int clientId in clientIds)
            {
                try
                {
                    var conflits = DetecterConflits(clientId, menuId);
                    tousLesConflits.AddRange(conflits);
                }
                catch (ApplicationException)
                {
                    // Client introuvable, on continue avec les autres
                    continue;
                }
            }

            return tousLesConflits;
        }

        /// <summary>
        /// Valide une réservation en vérifiant les conflits d'allergènes.
        /// </summary>
        /// <param name="request">Requête de réservation.</param>
        /// <returns>Réponse de validation avec les éventuels conflits.</returns>
        public ValidationReservationResponse ValiderReservation(ReservationRequest request)
        {
            ValidationReservationResponse response = new ValidationReservationResponse();

            // Détecter les conflits
            List<ConflitAllergene> conflits = DetecterConflits(request.ClientId, request.MenuId);

            response.Conflits = conflits;
            response.ADesConflits = conflits.Count > 0;

            if (!response.ADesConflits)
            {
                // Pas de conflits, réservation validée
                response.EstValide = true;
                response.Message = "Réservation validée. Aucun conflit d'allergène détecté.";
            }
            else if (request.ForceReservation)
            {
                // Le restaurateur force la réservation malgré les conflits
                if (string.IsNullOrWhiteSpace(request.NoteOverride))
                {
                    response.EstValide = false;
                    response.Message = "Une note explicative est requise pour forcer la réservation.";
                }
                else
                {
                    response.EstValide = true;
                    response.EstForcee = true;
                    response.NoteOverride = request.NoteOverride;
                    response.Message = $"Réservation forcée avec note : {request.NoteOverride}";
                }
            }
            else
            {
                // Conflits détectés, réservation bloquée
                response.EstValide = false;
                response.Message = "Réservation bloquée. Des conflits d'allergènes ont été détectés.";
            }

            return response;
        }

        /// <summary>
        /// Récupère les allergènes présents dans un menu.
        /// </summary>
        /// <param name="menuId">Identifiant du menu.</param>
        /// <returns>Liste des allergènes présents dans le menu.</returns>
        public List<Allergene> GetAllergenesParMenu(int menuId)
        {
            return _allergeneDAO.GetAllergenesParMenu(menuId);
        }

        /// <summary>
        /// Génère un message d'alerte formaté pour l'affichage.
        /// </summary>
        private string GenererMessageAlerte(Client client, List<Allergene> allergenes, List<Ingredient> ingredients)
        {
            string listeAllergenes = string.Join(", ", allergenes.Select(a => $"'{a.Nom}'"));
            string listeIngredients = string.Join(", ", ingredients.Select(i => $"'{i.Nom}'"));

            return $"Attention : Le menu contient {listeIngredients}. " +
                   $"{client.Prenom} {client.Nom} est allergique à {listeAllergenes}.";
        }
    }
}