using AwesomeAuditing.Domain;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace AwesomeAuditing.Infra.Data.Context.Initialization
{
    /// <summary>
    /// This class configure and initialize the database for AwesomeAuditing.
    /// </summary>
    internal class ContextInitializer
    {
        /// <summary>
        /// Updates the database creating the AwesomeAuditing table.
        /// </summary>
        /// <param name="dbContextName">Name of the database context.</param>
        public static void UpdateDatabase(string dbContextName)
        {
            AwesomeAuditingConfig.DBContextName = dbContextName;

            using (Context context = new Context())
            {
                bool awesomeAuditingTableExists = CheckIfAwesomeAuditingTableExists(context);
                if (!awesomeAuditingTableExists)
                    CreateAwesomeAuditingTable(context);
            }
        }

        /// <summary>
        /// Checks if AwesomeAuditing table exists.
        /// </summary>
        /// <param name="context">The databse context.</param>
        /// <returns>True if AwesomeAuditing table already exists.</returns>
        private static bool CheckIfAwesomeAuditingTableExists(Context context)
        {
            string tableExistsQuery = String.Format(@"
                         SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                         WHERE TABLE_CATALOG = '{0}' AND TABLE_NAME = '{1}'"
                         , context.Database.Connection.Database
                         , "AuditRecord");

            bool exists = context.Database
                     .SqlQuery<int?>(tableExistsQuery)
                     .SingleOrDefault() != null;

            return exists;
        }

        /// <summary>
        /// Creates the AwesomeAuditing table.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <exception cref="AwesomeAuditingException"></exception>
        private static void CreateAwesomeAuditingTable(Context context)
        {
            using (DbContextTransaction transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    context.Database.ExecuteSqlCommand(CreateTableQuery);
                    context.Database.ExecuteSqlCommand(CreateOperationIndexQuery);
                    context.Database.ExecuteSqlCommand(CreateUserNameIndexQuery);
                    context.Database.ExecuteSqlCommand(CreateEntityIndexQuery);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new AwesomeAuditingException(
                        String.Format(@"It was not possible to initalize the database creation or modification: {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// Gets the create table query.
        /// </summary>
        /// <value>
        /// The create table query.
        /// </value>
        public static string CreateTableQuery
        {
            get
            {
                return @"
                        CREATE TABLE [dbo].[AuditRecord](
	                        [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [EntityId] [nvarchar](100) NULL,
	                        [EntityType] [nvarchar](100) NOT NULL,
	                        [Operation] [int] NOT NULL,
	                        [UserName] [nvarchar](100) NOT NULL,
	                        [Timestamp] [datetime2](7) NOT NULL,
	                        [InformationInByte] [varbinary](max) NOT NULL DEFAULT (0x),
                         CONSTRAINT [PK_dbo.AuditRecord] PRIMARY KEY CLUSTERED 
                        (
	                        [Id] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            }
        }

        /// <summary>
        /// Gets the create operation index query.
        /// </summary>
        /// <value>
        /// The create operation index query.
        /// </value>
        public static string CreateOperationIndexQuery
        {
            get
            {
                return @"CREATE NONCLUSTERED INDEX [IX_Operation] ON [dbo].[AuditRecord]
                        (
	                        [Operation] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
            }
        }

        /// <summary>
        /// Gets the create user name index query.
        /// </summary>
        /// <value>
        /// The create user name index query.
        /// </value>
        public static string CreateUserNameIndexQuery
        {
            get
            {
                return @"CREATE NONCLUSTERED INDEX [IX_UserName] ON [dbo].[AuditRecord]
                        (
	                        [UserName] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
            }
        }

        /// <summary>
        /// Gets the create entity index query.
        /// </summary>
        /// <value>
        /// The create entity index query.
        /// </value>
        public static string CreateEntityIndexQuery
        {
            get
            {
                return @"CREATE UNIQUE NONCLUSTERED INDEX [IX_EntityType_EntityId_Timestamp] ON [dbo].[AuditRecord]
                        (
	                        [EntityType] ASC,
	                        [EntityId] ASC,
	                        [Timestamp] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
            }
        }
    }
}
