using System;

namespace AwesomeAuditing
{
    /// <summary>
    /// This parameter attribute should be applied to the boolean parameter that tells if the method will be audited or not.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MethodIsAuditableParam : Attribute
    { }
}
