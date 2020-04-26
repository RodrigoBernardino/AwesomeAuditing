
namespace AwesomeAuditing
{
    /// <summary>
    /// All auditable entities must inherit from this interface.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Gets the auditable entity identifier.
        /// </summary>
        /// <value>
        /// The auditable entity identifier.
        /// </value>
        int Id { get; }
    }
}
