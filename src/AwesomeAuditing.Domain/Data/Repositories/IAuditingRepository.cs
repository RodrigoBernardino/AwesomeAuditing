using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AwesomeAuditing.Data
{
    /// <summary>
    /// AwesomeAuditing repository class.
    /// </summary>
    public interface IAuditingRepository
    {
        /// <summary>
        /// Adds the specified audit record.
        /// </summary>
        /// <param name="auditRecord">The audit record.</param>
        void Add(AuditRecord auditRecord);
        /// <summary>
        /// Finds all saved audit records.
        /// </summary>
        /// <returns>The list of all saved audit records.</returns>
        IEnumerable<AuditRecord> FindAll();
        /// <summary>
        /// Finds all saved audit records with string clauses.
        /// </summary>
        /// <param name="clauses">The list of string clauses that will be applied in the search.</param>
        /// <returns>The list of all saved audit records according with the clauses.</returns>
        IEnumerable<AuditRecord> FindAll(IEnumerable<string> clauses);
        /// <summary>
        /// Finds all saved audit records with lambda expression clauses.
        /// </summary>
        /// <param name="clauses">The list of lambda expression clauses that will be applied in the search.</param>
        /// <returns>The list of all saved audit records according with the clauses.</returns>
        IEnumerable<AuditRecord> FindAll(params Expression<Func<AuditRecord, bool>>[] clauses);
        /// <summary>
        /// Finds all saved audit records with query limits.
        /// </summary>
        /// <param name="queryLimits">The query limits used for pagination and ordering.</param>
        /// <returns>The list of all saved audit records according with the query limits.</returns>
        IEnumerable<AuditRecord> FindAll(QueryLimits queryLimits);
        /// <summary>
        /// Finds all saved audit records with query limits and string clauses.
        /// </summary>
        /// <param name="queryLimits">The query limits used for pagination and ordering.</param>
        /// <param name="clauses">The list of string clauses that will be applied in the search.</param>
        /// <returns>The list of all saved audit records according with the query limits and clauses.</returns>
        IEnumerable<AuditRecord> FindAll(QueryLimits queryLimits, IEnumerable<string> clauses);
        /// <summary>
        /// Finds all saved audit records with query limits and lambda expression clauses.
        /// </summary>
        /// <param name="queryLimits">The query limits used for pagination and ordering.</param>
        /// <param name="clauses">The list of lambda expression clauses that will be applied in the search.</param>
        /// <returns>The list of all saved audit records according with the query limits and clauses.</returns>
        IEnumerable<AuditRecord> FindAll(QueryLimits queryLimits, params Expression<Func<AuditRecord, bool>>[] clauses);
        /// <summary>
        /// Counts all saved audit records.
        /// </summary>
        /// <returns>The number of saved audit records</returns>
        int Count();
        /// <summary>
        /// Counts all saved audit records with string clauses.
        /// </summary>
        /// <param name="clauses">The list of string clauses that will be applied in the count query.</param>
        /// <returns>The number of saved audit records according with the clauses.</returns>
        int Count(IEnumerable<string> clauses);
        /// <summary>
        /// Counts all saved audit records with lambda expression clauses.
        /// </summary>
        /// <param name="clauses">The list of lambda expression clauses that will be applied in the count query.</param>
        /// <returns>The number of saved audit records according with the clauses.</returns>
        int Count(params Expression<Func<AuditRecord, bool>>[] clauses);
        /// <summary>
        /// Gets the last entity's audit record value.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="timestamp">(Optional) The timestamp used to limit the search till it.</param>
        /// <returns>The entity's last state.</returns>
        AuditRecord GetLastValue(int id, string entityType, DateTime? timestamp = null);
    }
}
