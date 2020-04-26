using System;

namespace AwesomeAuditing
{
    /// <summary>
    /// AwesomeAuditing exception class.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AwesomeAuditingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AwesomeAuditingException"/> class.
        /// </summary>
        public AwesomeAuditingException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AwesomeAuditingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AwesomeAuditingException(string message)
            : base(message)
        { }
    }
}
