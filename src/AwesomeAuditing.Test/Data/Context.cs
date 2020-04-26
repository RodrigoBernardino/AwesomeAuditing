using AwesomeAuditing.Test.Data.Mapping;
using AwesomeAuditing.Test.Domain.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace AwesomeAuditing.Test.Data
{
    public class Context : DbContext
    {
        public Context()
            : base("TestContext")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RoleMap());
        }
    }
}
