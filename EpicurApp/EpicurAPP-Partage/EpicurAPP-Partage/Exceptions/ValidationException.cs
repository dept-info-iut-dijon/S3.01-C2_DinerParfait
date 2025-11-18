namespace EpicurAPP_Partage.Exceptions
{
    /// <summary>
    /// Exception levée lorsqu'une validation échoue.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Nom du champ en erreur.
        /// </summary>
        public string? FieldName { get; }

        /// <summary>
        /// Initialise une nouvelle instance de ValidationException.
        /// </summary>
        /// <param name="message">Message d'erreur de validation.</param>
        public ValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de ValidationException avec le nom du champ.
        /// </summary>
        /// <param name="fieldName">Nom du champ en erreur.</param>
        /// <param name="message">Message d'erreur de validation.</param>
        public ValidationException(string fieldName, string message) : base(message)
        {
            FieldName = fieldName;
        }
    }
}
