using System;

namespace AwesomeAuditing.Domain.Utils.Extensions
{
    internal static class ExceptionExtensions
    {
        public static string GetRealExceptionMessage(this Exception ex)
        {
            while (true)
            {
                if (ex.InnerException == null) return ex.Message;
                ex = ex.InnerException;
            }
        }
    }
}
