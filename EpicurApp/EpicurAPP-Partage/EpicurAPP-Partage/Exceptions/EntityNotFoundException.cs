namespace EpicurAPP_Partage.Exceptions
{
    /// <summary>
    /// Exception levée lorsqu'une entité recherchée n'est pas trouvée dans la base de données.
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Nom de l'entité non trouvée.
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Identifiant de l'entité recherchée.
        /// </summary>
        public int EntityId { get; }

        /// <summary>
        /// Initialise une nouvelle instance de EntityNotFoundException.
        /// </summary>
        /// <param name="entityName">Nom de l'entité non trouvée.</param>
        /// <param name="id">Identifiant recherché.</param>
        public EntityNotFoundException(string entityName, int id)
            : base($"{entityName} avec l'ID {id} introuvable.")
        {
            EntityName = entityName;
            EntityId = id;
        }

        /// <summary>
        /// Initialise une nouvelle instance de EntityNotFoundException avec un message personnalisé.
        /// </summary>
        /// <param name="entityName">Nom de l'entité non trouvée.</param>
        /// <param name="id">Identifiant recherché.</param>
        /// <param name="message">Message d'erreur personnalisé.</param>
        public EntityNotFoundException(string entityName, int id, string message) : base(message)
        {
            EntityName = entityName;
            EntityId = id;
        }
    }
}
