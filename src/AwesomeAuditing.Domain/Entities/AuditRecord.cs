using AwesomeAuditing.Domain.Utils;
using System;
using System.Text;

namespace AwesomeAuditing
{
    /// <summary>
    /// AwesomeAuditing audit record.
    /// </summary>
    public sealed class AuditRecord
    {
        /// <summary>
        /// Gets or sets the audit record identifier.
        /// </summary>
        /// <value>
        /// The audited record identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the audited entity identifier.
        /// </summary>
        /// <value>
        /// The audited entity identifier.
        /// </value>
        public string EntityId { get; set; }
        /// <summary>
        /// Gets or sets the type of the audited entity.
        /// </summary>
        /// <value>
        /// The type of the audited entity.
        /// </value>
        public string EntityType { get; set; }
        /// <summary>
        /// Gets or sets the audit operation.
        /// </summary>
        /// <value>
        /// The audit operation.
        /// </value>
        public AuditOperation Operation { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the audit timestamp.
        /// </summary>
        /// <value>
        /// The audit timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the audited information.
        /// </summary>
        /// <value>
        /// The audited information.
        /// </value>
        public string Information  { get; set; }

        /// <summary>
        /// Gets or sets the audited information in byte.
        /// </summary>
        /// <value>
        /// The audited information in byte.
        /// </value>
        internal byte[] InformationInByte { get; set; }
        /// <summary>
        /// Converts the audited information in byte to json (string).
        /// </summary>
        internal void ConvertInformationToJSON() 
        {
            Information = Encoding.UTF8.GetString(Compressor.Decompress(InformationInByte));
        }
    }
}
