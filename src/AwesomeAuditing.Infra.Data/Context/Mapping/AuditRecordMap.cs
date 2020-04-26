using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AwesomeAuditing.Infra.Data.Context.Mapping
{
    /// <summary>
    /// The Entity Framework fluent API mapping for the AuditRecord entity.
    /// </summary>
    /// <seealso cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{AwesomeAuditing.AuditRecord}" />
    internal class AuditRecordMap : EntityTypeConfiguration<AuditRecord>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRecordMap"/> class.
        /// </summary>
        public AuditRecordMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(100);
            Property(t => t.EntityId)
                .HasMaxLength(100);
            Property(t => t.InformationInByte)
                .IsRequired();
            Property(t => t.Operation)
                .IsRequired();
            Property(t => t.Timestamp)
                .IsRequired()
                .HasColumnType("datetime2");
            Property(t => t.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            Ignore(t => t.Information);

            HasIndex(t => new { t.Operation });

            HasIndex(t => new { t.UserName });

            HasIndex(t => new { t.EntityType, t.EntityId, t.Timestamp })
                .IsUnique();
        }
    }
}
