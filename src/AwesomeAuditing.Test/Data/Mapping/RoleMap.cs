using AwesomeAuditing.Test.Domain.Entities;
using System.Data.Entity.ModelConfiguration;

namespace AwesomeAuditing.Test.Data.Mapping
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            //relations
            HasMany(t => t.Users)
                .WithMany(t => t.Roles)
                .Map(t =>
                {
                    t.MapLeftKey("RoleId");
                    t.MapRightKey("UserId");
                    t.ToTable("Role_User");
                });
        }
    }
}
