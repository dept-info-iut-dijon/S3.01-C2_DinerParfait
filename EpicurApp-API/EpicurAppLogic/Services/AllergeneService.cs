using EpicurAPP_Partage.Exceptions;
using EpicurAppLogic.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    /// <summary>
    /// implementation des services allergene 
    /// </summary>
    public class AllergeneService : IAllergeneService
    {
        private readonly IAllergeneDAO _allergeneDAO;

        /// <summary>
        /// Constructeur de la classe AllergeneService
        /// </summary>
        /// <param name="allergeneDAO">Le dao de l'allergene</param>
        public AllergeneService(IAllergeneDAO allergeneDAO)
        {
            _allergeneDAO = allergeneDAO;
        }

        /// <summary>
        /// Méthode pour récupérer tous les allergènes
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Recupération impossible de l'allergene</exception>
        public List<Allergene> GetAll()
        {
            try
            {
                return _allergeneDAO.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des allergènes.", ex);
            }
        }

        /// <summary>
        /// Méthode pour récupérer les allergènes d'un client
        /// </summary>
        /// <param name="clientId">id du client</param>
        /// <returns>La liste d'allergies du client</returns>
        /// <exception cref="ApplicationException">Recupération impossible des allergies du client</exception>
        public List<Allergene> GetAllergenesByClient(int clientId)
        {
            try
            {
                return _allergeneDAO.GetAllergenesByClient(clientId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des allergènes du client.", ex);
            }
        }

        /// <summary>
        /// Méthode pour ajouter des allergènes à un client
        /// </summary>
        /// <param name="clientId">id du client</param>
        /// <param name="allergeneIds">id des allergene du client</param>
        /// <exception cref="ApplicationException">Ajout impossible des allergenes</exception>
        public void AjouterAllergenesAuClient(int clientId, List<int> allergeneIds)
        {
            if (allergeneIds == null || allergeneIds.Count == 0)
            {
                throw new InvalidFieldException("La liste des allergènes ne peut pas être vide.");
            }

            try
            {
                _allergeneDAO.AjouterAllergenesAuClient(clientId, allergeneIds);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'ajout des allergènes au client.", ex);
            }
        }

        /// <summary>
        /// Ajoute un nouvel allergène
        /// </summary>
        /// <param name="allergene">allegene a ajouter</param>
        /// <exception cref="InvalidFieldException">Le nom de l'allergene ne peut etre vide</exception>
        /// <exception cref="ApplicationException">Impossible d'ajouter l'allergene </exception>
        public void AjouterAllergene(Allergene allergene)
        {
            if (string.IsNullOrWhiteSpace(allergene.Nom))
            {
                throw new InvalidFieldException("Le nom de l'allergène est obligatoire.");
            }

            try
            {
                _allergeneDAO.AjouterAllergene(allergene);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'ajout de l'allergène.", ex);
            }
        }
    }
}
