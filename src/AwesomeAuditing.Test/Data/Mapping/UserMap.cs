using AwesomeAuditing.Test.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace AwesomeAuditing.Test.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
