using AwesomeAuditing.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace AwesomeAuditing.Data
{
    /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
    public class AuditingRepository : IAuditingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditingRepository"/> class.
        /// </summary>
        public AuditingRepository()
        {
            ContextCreator = () =>
            {
                var context = new Context();
                ConfigContext(context);
                return context;
            };
        }

        /// <summary>
        /// Gets or sets the context creator function.
        /// </summary>
        /// <value>
        /// The context creator function.
        /// </value>
        private Func<Context> ContextCreator { get; set; }

        /// <summary>
        /// Configurations the context.
        /// </summary>
        /// <param name="context">The context.</param>
        private static void ConfigContext(DbContext context)
        {
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual void Add(AuditRecord auditRecord)
        {
            using (var entityContext = ContextCreator())
            {
                entityContext.Set<AuditRecord>().Add(auditRecord);
                entityContext.SaveChanges();
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual IEnumerable<AuditRecord> FindAll()
        {
            using (var entityContext = ContextCreator())
            {
                List<AuditRecord> records = entityContext.Set<AuditRecord>().ToList();

                records.ForEach(r => r.ConvertInformationToJSON());
                return records;
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual IEnumerable<AuditRecord> FindAll(IEnumerable<string> clauses)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<AuditRecord> temporaryQuery = entityContext.Set<AuditRecord>();
                temporaryQuery = clauses.Aggregate(temporaryQuery, (current, clause) => current.Where(clause));
                List<AuditRecord> records = temporaryQuery.ToList();

                records.ForEach(r => r.ConvertInformationToJSON());
                return records;
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual IEnumerable<AuditRecord> FindAll(params Expression<Func<AuditRecord, bool>>[] clauses)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<AuditRecord> temporaryQuery = entityContext.Set<AuditRecord>();
                temporaryQuery = clauses.Aggregate(temporaryQuery, (current, clause) => current.Where(clause));
                List<AuditRecord> records = temporaryQuery.ToList();

                records.ForEach(r => r.ConvertInformationToJSON());
                return records;
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual IEnumerable<AuditRecord> FindAll(QueryLimits queryLimits)
        {
            using (var entityContext = ContextCreator())
            {
                List<AuditRecord> records = entityContext.Set<AuditRecord>().OrderBy(String.Format("{0} {1}", queryLimits.Order, queryLimits.Orientation))
                    .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                    .Take(queryLimits.Limit)
                    .ToList();

                records.ForEach(r => r.ConvertInformationToJSON());
                return records;
            }
        }

        //// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual IEnumerable<AuditRecord> FindAll(QueryLimits queryLimits, IEnumerable<string> clauses)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<AuditRecord> temporaryQuery = entityContext.Set<AuditRecord>();
                temporaryQuery = clauses.Aggregate(temporaryQuery, (current, clause) => current.Where(clause));
                List<AuditRecord> records = temporaryQuery.OrderBy(String.Format("{0} {1}", queryLimits.Order, queryLimits.Orientation))
                    .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                    .Take(queryLimits.Limit)
                    .ToList();

                records.ForEach(r => r.ConvertInformationToJSON());
                return records;
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual IEnumerable<AuditRecord> FindAll(QueryLimits queryLimits, params Expression<Func<AuditRecord, bool>>[] clauses)
        {
            using (var entityContext = ContextCreator())
            {
                IQueryable<AuditRecord> temporaryQuery = entityContext.Set<AuditRecord>();
                temporaryQuery = clauses.Aggregate(temporaryQuery, (current, clause) => current.Where(clause));
                List<AuditRecord> records = temporaryQuery.OrderBy(String.Format("{0} {1}", queryLimits.Order, queryLimits.Orientation))
                    .Skip(queryLimits.Limit * (queryLimits.Page - 1))
                    .Take(queryLimits.Limit)
                    .ToList();

                records.ForEach(r => r.ConvertInformationToJSON());
                return records;
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual int Count()
        {
            using (var context = ContextCreator())
            {
                try
                {
                    return context.Set<AuditRecord>().Count();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        String.Format("It was not possible to find the data: {0}", ex.Message));
                }
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual int Count(IEnumerable<string> clauses)
        {
            using (var context = ContextCreator())
            {
                try
                {
                    IQueryable<AuditRecord> temporaryQuery = context.Set<AuditRecord>();
                    temporaryQuery = clauses.Aggregate(temporaryQuery, (current, clause) => current.Where(clause));
                    return temporaryQuery.Count();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        String.Format("It was not possible to find the data: {0}", ex.Message));
                }
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public virtual int Count(params Expression<Func<AuditRecord, bool>>[] clauses)
        {
            using (var context = ContextCreator())
            {
                try
                {
                    IQueryable<AuditRecord> temporaryQuery = context.Set<AuditRecord>();
                    temporaryQuery = clauses.Aggregate(temporaryQuery, (current, clause) => current.Where(clause));
                    return temporaryQuery.Count();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        String.Format("It was not possible to find the data: {0}", ex.Message));
                }
            }
        }

        /// <inheritdoc cref="AwesomeAuditing.Data.IAuditingRepository"/>
        public AuditRecord GetLastValue(int id, string entityType, DateTime? timestamp = null)
        {
            timestamp = timestamp == null ? DateTime.Now : timestamp;

            using (var context = ContextCreator())
            {
                var lastValueQuery = context.Set<AuditRecord>()
                        .Where(a => a.EntityId == id.ToString())
                        .Where(a => a.EntityType == entityType)
                        .Where(a => a.Timestamp < timestamp)
                        .OrderByDescending(a => a.Timestamp);

                AuditRecord lastValue = lastValueQuery.FirstOrDefault();
                if (lastValue != null)
                {
                    lastValue.ConvertInformationToJSON();
                }
                return lastValue;
            }
        }
    }
}
