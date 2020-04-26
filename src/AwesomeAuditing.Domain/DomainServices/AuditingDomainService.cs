using AwesomeAuditing.Data;
using AwesomeAuditing.Domain.Utils;
using AwesomeAuditing.Domain.Utils.Extensions;
using System;
using System.Text;

namespace AwesomeAuditing.Domain.DomainServices
{
    /// <summary>
    /// AwesomeAuditing domain service class.
    /// </summary>
    internal class AuditingDomainService
    {
        /// <summary>
        /// The AwesomeAuditing repository.
        /// </summary>
        private readonly IAuditingRepository _auditingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditingDomainService"/> class.
        /// </summary>
        /// <param name="auditingHandler">The AwesomeAuditing user handler.</param>
        /// <param name="auditingRepository">The AwesomeAuditing repository.</param>
        public AuditingDomainService(IAuditingRepository auditingRepository)
        {
            _auditingRepository = auditingRepository;
        }

        /// <summary>
        /// Audits the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="operation">The auditing operation.</param>
        /// <param name="userName">Name of the user.</param>
        public void Audit(IAuditableEntity entity, AuditOperation operation, string userName)
        {
            _auditingRepository.Add(new AuditRecord
            {
                Timestamp = DateTime.Now,
                EntityType = entity.GetType().Name,
                InformationInByte = GetInformationFile(entity),
                EntityId = entity.Id.ToString(),
                Operation = operation,
                UserName = userName
            });
        }

        /// <summary>
        /// Gets the information file.
        /// </summary>
        /// <param name="entity">The entity to be audited.</param>
        /// <returns>The serialized entity converted in byte array.</returns>
        private byte[] GetInformationFile(IAuditableEntity entity)
        {
            byte[] informationFile = Encoding.UTF8.GetBytes(entity.ConvertToJsonWithNestedProperties(AwesomeAuditingConfig.CamelCaseSerialization));

            return Compressor.Compress(informationFile);
        }
    }
}
