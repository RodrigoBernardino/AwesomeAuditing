using AwesomeAuditing.Domain;
using AwesomeAuditing.Infra.Data.Context.Mapping;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Text.RegularExpressions;

namespace AwesomeAuditing.Infra.Data.Context
{
    /// <summary>
    /// The AwesomeAuditing Entity Framework context class.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    internal class Context : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        public Context()
            : base(AwesomeAuditingConfig.DBContextName)
        {
            Database.SetInitializer<Context>(null);
        }

        internal DbSet<AuditRecord> AuditRecords { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuidler, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new AuditRecordMap());
        }

        /// <summary>
        /// Gets the name of the table corresponding to the method's generic type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The table name in string.</returns>
        public string GetTableName<T>()
            where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)this).ObjectContext;

            string sql = objectContext.CreateObjectSet<T>().ToTraceString();
            Regex regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;

            return table;
        }
    }
}
