using AwesomeAuditing.Domain;

namespace AwesomeAuditing.Initialization
{
    /// <summary>
    /// This class holds the custom AwesomeAuditing user handler.
    /// </summary>
    /// <seealso cref="AwesomeAuditing.Domain.IAuditingHandler" />
    internal class AuditingHandler : IAuditingHandler
    {
        /// <summary>
        /// Gets the auditing user handler.
        /// </summary>
        /// <value>
        /// The auditing user handler.
        /// </value>
        public AwesomeAuditingHandler AwesomeAuditingHandler { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Initialization.AuditingHandler"/> class.
        /// </summary>
        /// <param name="auditingUserHandler">The AwesomeAuditing user handler.</param>
        public AuditingHandler(AwesomeAuditingHandler auditingUserHandler)
        {
            AwesomeAuditingHandler = auditingUserHandler;
        }
    }
}
