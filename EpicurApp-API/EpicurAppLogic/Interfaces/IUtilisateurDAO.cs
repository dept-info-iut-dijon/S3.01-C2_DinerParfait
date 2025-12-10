using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Interfaces
{
    /// <summary>
    /// Interface pour accéder à la base de données des utilisateurs.
    /// Fournit les opérations pour l'authentification et la gestion des utilisateurs.
    /// </summary>
    public interface IUtilisateurDAO
    {
        /// <summary>
        /// Récupère un utilisateur par son email.
        /// </summary>
        /// <param name="email">Email de l'utilisateur.</param>
        /// <returns>L'utilisateur trouvé ou null si non trouvé.</returns>
        Utilisateur? GetByEmail(string email);

        /// <summary>
        /// Récupère un utilisateur par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur.</param>
        /// <returns>L'utilisateur trouvé ou null si non trouvé.</returns>
        Utilisateur? GetById(int id);

        /// <summary>
        /// Récupère tous les utilisateurs d'un restaurant.
        /// </summary>
        /// <param name="restaurantId">Identifiant du restaurant.</param>
        /// <returns>Liste des utilisateurs du restaurant.</returns>
        List<Utilisateur> GetByRestaurantId(int restaurantId);

        /// <summary>
        /// Ajoute un nouvel utilisateur dans la base de données.
        /// </summary>
        /// <param name="utilisateur">L'utilisateur à ajouter.</param>
        void AjouterUtilisateur(Utilisateur utilisateur);

        /// <summary>
        /// Modifie un utilisateur existant.
        /// </summary>
        /// <param name="utilisateur">L'utilisateur avec les données mises à jour.</param>
        void ModifierUtilisateur(Utilisateur utilisateur);

        /// <summary>
        /// Supprime un utilisateur de la base de données.
        /// </summary>
        /// <param name="id">Identifiant de l'utilisateur à supprimer.</param>
        void SupprimerUtilisateur(int id);
    }
}
