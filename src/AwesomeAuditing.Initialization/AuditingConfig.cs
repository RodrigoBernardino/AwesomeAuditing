using AwesomeAuditing.Domain;
using AwesomeAuditing.Infra.Data.Context.Initialization;
using AwesomeAuditing.Initialization.Bootstrapper;

namespace AwesomeAuditing
{
    /// <summary>
    /// Configuration class used to initialize the awesome auditing solution.
    /// </summary>
    /// <remarks>
    /// Today this configuration only works with the Autofac DI container.
    /// </remarks>
    public class AuditingConfig
    {
        /// <summary>
        /// Updates the solution's database and configures the auditing module in the autofac DI container.
        /// </summary>
        /// <param name="dbContextName">Name of the database context.</param>
        /// <param name="auditingUserHandler">The custom AwesomeAuditing user handler that will provide the action's user at runtime.</param>
        /// <param name="camelCaseSerialization">(Optional) Informs if the auditing information will be serialized using the camel case pattern. Default: false</param>
        /// <returns>The configured autofac module.</returns>
        public static AuditingModule CreateAuditingModule(string dbContextName, AwesomeAuditingHandler auditingUserHandler, bool camelCaseSerialization = false)
        {
            AwesomeAuditingConfig.CamelCaseSerialization = camelCaseSerialization;
            ContextInitializer.UpdateDatabase(dbContextName);

            return new AuditingModule(auditingUserHandler);
        }
    }
}
