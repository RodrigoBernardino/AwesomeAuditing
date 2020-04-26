
namespace AwesomeAuditing.Domain
{
    /// <summary>
    /// Static class that holds general AwesomeAuditing configuration properties.
    /// </summary>
    internal static class AwesomeAuditingConfig
    {
        /// <summary>
        /// Gets or sets the name of the database context.
        /// </summary>
        /// <value>
        /// The name of the database context.
        /// </value>
        public static string DBContextName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether serialization will use the camel case pattern.
        /// </summary>
        /// <value>
        ///   <c>true</c> if camel case serialization; Otherwise, the default object serialization will be used.<c>false</c>.
        /// </value>
        public static bool CamelCaseSerialization { get; set; }
    }
}
