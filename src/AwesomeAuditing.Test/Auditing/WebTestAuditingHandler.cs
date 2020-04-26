using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace AwesomeAuditing.Test.Auditing
{
    public class WebTestAuditingHandler : AwesomeAuditingHandler
    {
        public override string GetAuditingUser()
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }

        public override IEnumerable<string> GetAlreadyInterceptedMethods()
        {
            CheckIfCurrentHttpContextAuditingItemExistsAndCreateIt();
            return (ConcurrentBag<string>)HttpContext.Current.Items["AuditingInteceptedMethods"];
        }

        public override void AddInterceptedMethod(string interceptedMethodUniqueId)
        {
            CheckIfCurrentHttpContextAuditingItemExistsAndCreateIt();
            ConcurrentBag<string> interceptedMethods = (ConcurrentBag<string>)HttpContext.Current.Items["AuditingInteceptedMethods"];
            interceptedMethods.Add(interceptedMethodUniqueId);
        }

        private void CheckIfCurrentHttpContextAuditingItemExistsAndCreateIt()
        {
            if (HttpContext.Current.Items["AuditingInteceptedMethods"] == null)
                HttpContext.Current.Items["AuditingInteceptedMethods"] = new ConcurrentBag<string>();
        }
    }
}
