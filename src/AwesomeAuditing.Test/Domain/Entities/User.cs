using AwesomeAuditing.Test.Data.Utils;
using System.Collections.Generic;

namespace AwesomeAuditing.Test.Domain.Entities
{
    public class User : IIdentifiableEntity, IAuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
