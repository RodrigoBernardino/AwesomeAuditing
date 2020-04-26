using System;

namespace AwesomeAuditing
{
    /// <summary>
    /// This method attribute should be applied to methods that do operations that need to be audited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class Audit : Attribute
    {
        /// <summary>
        /// The suditing operation
        /// </summary>
        internal AuditOperation Operation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Audit"/> class.
        /// </summary>
        /// <param name="operation">The auditing operation.</param>
        public Audit(AuditOperation operation)
        {
            Operation = operation;
        }
    }
}
