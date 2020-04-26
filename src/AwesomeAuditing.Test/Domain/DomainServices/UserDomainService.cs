using AwesomeAuditing.Test.Data.Repositories;
using AwesomeAuditing.Test.Domain.Entities;
using System.Runtime.CompilerServices;

namespace AwesomeAuditing.Test.Domain.DomainServices
{
    public class UserDomainService : IUserDomainService
    {
        private readonly IEntityRepository<User> _userRepository;
        public UserDomainService(IEntityRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [Audit(AuditOperation.Create), MethodImpl(MethodImplOptions.NoInlining)]
        public virtual User Add(User newUser, [MethodIsAuditableParam]bool methodIsAuditable = true)
        {
            return _userRepository.Add(newUser);
        }

        [Audit(AuditOperation.Update), MethodImpl(MethodImplOptions.NoInlining)]
        public virtual User Update(User user, [MethodIsAuditableParam]bool methodIsAuditable = true)
        {
            return _userRepository.Update(user);
        }

        [Audit(AuditOperation.Remove), MethodImpl(MethodImplOptions.NoInlining)]
        public virtual User Remove(User user, [MethodIsAuditableParam]bool methodIsAuditable = true)
        {
            return _userRepository.Remove(user);
        }
    }
}
