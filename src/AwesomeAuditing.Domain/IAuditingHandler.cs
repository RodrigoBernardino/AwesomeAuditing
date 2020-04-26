
namespace AwesomeAuditing.Domain
{
    /// <summary>
    /// AwesomeAuditing handler that holds the AwesomeAuditing user handler.
    /// </summary>
    internal interface IAuditingHandler
    {
        /// <summary>
        /// Gets the AwesomeAuditing user handler.
        /// </summary>
        /// <value>
        /// The AwesomeAuditing user handler.
        /// </value>
        AwesomeAuditingHandler AwesomeAuditingHandler { get; }
    }
}
