using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Models;
using EpicurAppLogic.Interfaces;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// Service pour la gestion des idées de plats.
    /// </summary>
    public class IdeePlatService : IIdeePlatService
    {
        private readonly IIdeePlatDAO _ideePlatRepository;

        /// <summary>
        /// Constructeur de la classe IdeePlatService.
        /// </summary>
        /// <param name="ideePlatRepository">Le DAO pour interagir avec les idées de plats.</param>
        public IdeePlatService(IIdeePlatDAO ideePlatRepository)
        {
            _ideePlatRepository = ideePlatRepository;
        }

        /// <summary>
        /// Récupère toutes les idées de plats.
        /// </summary>
        /// <returns>Liste de toutes les idées de plats.</returns>
        public List<IdeePlat> ObtenirToutesLesIdees()
        {
            try
            {
                return _ideePlatRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des idées de plats.", ex);
            }
        }

        /// <summary>
        /// Récupère une idée de plat par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'idée de plat.</param>
        /// <returns>L'idée de plat correspondante.</returns>
        public IdeePlat? ObtenirIdeeParId(int id)
        {
            try
            {
                return _ideePlatRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erreur lors de la récupération de l'idée de plat {id}.", ex);
            }
        }

        /// <summary>
        /// Ajoute une nouvelle idée de plat.
        /// </summary>
        /// <param name="idee">L'idée de plat à ajouter.</param>
        public void AjouterIdee(IdeePlat idee)
        {
            if (string.IsNullOrWhiteSpace(idee.Titre))
            {
                throw new InvalidFieldException("Le titre de l'idée de plat est obligatoire.");
            }

            try
            {
                _ideePlatRepository.Ajouter(idee);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'ajout de l'idée de plat.", ex);
            }
        }

        /// <summary>
        /// Modifie une idée de plat existante.
        /// </summary>
        /// <param name="idee">L'idée de plat avec les données mises à jour.</param>
        public void ModifierIdee(IdeePlat idee)
        {
            if (string.IsNullOrWhiteSpace(idee.Titre))
            {
                throw new InvalidFieldException("Le titre de l'idée de plat est obligatoire.");
            }

            try
            {
                _ideePlatRepository.Modifier(idee);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la modification de l'idée de plat.", ex);
            }
        }

        /// <summary>
        /// Supprime une idée de plat.
        /// </summary>
        /// <param name="id">Identifiant de l'idée de plat à supprimer.</param>
        public void SupprimerIdee(int id)
        {
            try
            {
                _ideePlatRepository.Supprimer(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erreur lors de la suppression de l'idée de plat {id}.", ex);
            }
        }
    }
}
