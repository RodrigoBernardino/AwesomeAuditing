using AwesomeAuditing.Test.Domain.Entities;

namespace AwesomeAuditing.Test.Domain.DomainServices
{
    public interface IUserDomainService
    {
        User Add(User newUser, [MethodIsAuditableParam]bool methodIsAuditable = true);
        User Update(User user, [MethodIsAuditableParam]bool methodIsAuditable = true);
        User Remove(User user, [MethodIsAuditableParam]bool methodIsAuditable = true);
    }
}
