namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Classe pour lier la case à cocher au client dans l'IHM.
    /// Utilisée pour la sélection des invités pour les étiquettes nominatives.
    /// </summary>
    public class InviteSelection
    {
        /// <summary>
        /// Le client sélectionné.
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Indique si le client est sélectionné.
        /// </summary>
        public bool EstSelectionne { get; set; }

        /// <summary>
        /// Nom du client .
        /// </summary>
        public string Nom => Client?.Nom;

        /// <summary>
        /// Prénom du client.
        /// </summary>
        public string Prenom => Client?.Prenom;
    }
}
