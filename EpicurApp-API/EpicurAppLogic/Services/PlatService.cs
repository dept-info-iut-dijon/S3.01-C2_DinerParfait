using EpicurApp_API.Models;
using EpicurAPP_Partage.Exceptions;
using EpicurAPP_Partage.Interfaces;
using System;
using System.Collections.Generic;

namespace EpicurApp.Logic.Services
{
    public class PlatService : IPlatService
    {
        private readonly IPlatDAO _platDAO;

        public PlatService(IPlatDAO platDAO)
        {
            _platDAO = platDAO;
        }

        /// <summary>
        /// Récupère tous les plats.
        /// </summary>
        /// <returns>Liste de plats</returns>
        public List<Plat> ObtenirTousLesPlats()
        {
            try
            {
                return _platDAO.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération des plats.", ex);
            }
        }

        /// <summary>
        /// Récupère un plat par son Id.
        /// </summary>
        /// <param name="id">Identifiant du plat</param>
        /// <returns>Plat correspondant</returns>
        public Plat ObtenirPlatParId(int id)
        {
            try
            {
                var plat = _platDAO.GetById(id);
                if (plat == null)
                {
                    throw new Exception($"Le plat avec l'id {id} n'existe pas.");
                }
                return plat;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de la récupération du plat.", ex);
            }
        }

        /// <summary>
        /// Ajoute un nouveau plat.
        /// </summary>
        /// <param name="plat">Plat à ajouter</param>
        public void AjouterPlat(Plat plat)
        {
            if (string.IsNullOrWhiteSpace(plat.Nom))
            {
                throw new InvalidFieldException("Le nom du plat est obligatoire.");
            }

            try
            {
                _platDAO.Add(plat);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erreur lors de l'ajout du plat.", ex);
            }
        }
    }
}

