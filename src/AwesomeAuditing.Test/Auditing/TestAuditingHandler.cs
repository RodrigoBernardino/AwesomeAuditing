using System.Threading;

namespace AwesomeAuditing.Test.Auditing
{
    public class TestAuditingHandler : AwesomeAuditingHandler
    {
        public override string GetAuditingUser()
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }
    }
}
