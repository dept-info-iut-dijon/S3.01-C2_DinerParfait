using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using EpicurAPP_Partage.Models;

namespace EpicurAppLogic.Services
{
    public class AllergeneService : IAllergeneService
    {
        private readonly IAllergeneDAO _allergeneDAO;

        public AllergeneService(IAllergeneDAO allergeneDAO)
        {
            _allergeneDAO = allergeneDAO;
        }

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
