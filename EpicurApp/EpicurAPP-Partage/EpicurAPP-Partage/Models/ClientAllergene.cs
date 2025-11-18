namespace EpicurAPP_Partage.Models
{
    /// <summary>
    /// Table de liaison entre Client et Allergene
    /// </summary>
    public class ClientAllergene
    {
        public int ClientId { get; set; }
        public int AllergeneId { get; set; }
    }
}
