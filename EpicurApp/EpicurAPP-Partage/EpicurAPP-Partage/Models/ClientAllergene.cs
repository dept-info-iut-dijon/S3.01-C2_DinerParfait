namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Table de liaison entre Client et Allergene
    /// </summary>
    public class ClientAllergene
    {
        /// <summary>
        /// Identifiant du client
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// Identifiant de l'allergene
        /// </summary>
        public int AllergeneId { get; set; }
    }
}
