using AwesomeAuditing.Domain;
using AwesomeAuditing.Domain.DomainServices;
using AwesomeAuditing.Domain.Utils.Extensions;
using Castle.DynamicProxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AwesomeAuditing
{
    /// <summary>
    /// The AwesomeAuditing Castle DynamicProxy interceptor.
    /// </summary>
    /// <seealso cref="Castle.DynamicProxy.IInterceptor" />
    public sealed class AwesomeAuditingInterceptor : IInterceptor
    {
        /// <summary>
        /// The AwesomeAuditing user handler.
        /// </summary>
        private readonly IAuditingHandler _auditingHandler;
        /// <summary>
        /// The AwesomeAuditing domain service.
        /// </summary>
        private readonly AuditingDomainService _auditingDomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AwesomeAuditingInterceptor"/> class.
        /// </summary>
        /// <param name="auditingHandler">The AwesomeAuditing user handler.</param>
        /// <param name="auditingDomainService">The AwesomeAuditing domain service.</param>
        internal AwesomeAuditingInterceptor(IAuditingHandler auditingHandler
            , AuditingDomainService auditingDomainService)
        {
            _auditingHandler = auditingHandler;
            _auditingDomainService = auditingDomainService;
        }

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <exception cref="AwesomeAuditing.AwesomeAuditingException">
        /// The audited method's return type most inherit the 
        ///                     IAuditableEntity interface.
        /// or
        /// </exception>
        public void Intercept(IInvocation invocation)
        {
            if (!CheckIfMethodShouldBeIntercepted(invocation.Method, invocation.Arguments))
            {
                invocation.Proceed();
                return;
            }

            if (!CheckIfMethodReturnTypeInheritAuditEntity(invocation.Method))
                throw new AwesomeAuditingException(@"The audited method's return type most inherit the 
                    IAuditableEntity interface.");

            invocation.Proceed();

            if (CheckIfMethodIsAsync(invocation.Method))
                invocation.ReturnValue = AsyncProceedExecution((dynamic)invocation.ReturnValue, invocation);
            else
                AfterProceedExecution(invocation.ReturnValue, invocation);
        }

        /// <summary>
        /// Proceed the auditing flow for the async intercepted method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The intercepted method return task.</param>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        private async Task<T> AsyncProceedExecution<T>(Task<T> task, IInvocation invocation)
        {
            T result = await task.ConfigureAwait(false);
            AfterProceedExecution(result, invocation);

            return result;
        }

        /// <summary>
        /// Audit the intercepted method's return value after method execution completes.
        /// </summary>
        /// <param name="auditableEntity">The auditable entity.</param>
        /// <param name="invocation">The invocation.</param>
        /// <exception cref="AwesomeAuditing.AwesomeAuditingException"></exception>
        private void AfterProceedExecution(object auditableEntity, IInvocation invocation)
        {
            try
            {
                AuditReturnValue(auditableEntity, invocation.Method);
            }
            catch (Exception ex)
            {
                throw new AwesomeAuditingException(String.Format(@"It was not possible to audit the 
                    operation: {0}", ex.GetRealExceptionMessage()));
            }
        }

        /// <summary>
        /// Checks if method is asynchronous.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns><c>true</c> if method is asynchronous.</returns>
        private bool CheckIfMethodIsAsync(MethodInfo methodInfo)
        {
            return (methodInfo.ReturnType == typeof(Task) 
                || (methodInfo.ReturnType.IsGenericType 
                    && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
        }

        /// <summary>
        /// Checks if method should be intercepted.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="methodParamsObj">The method parameters object array.</param>
        /// <returns><c>true</c> if method should be intercepted.</returns>
        private bool CheckIfMethodShouldBeIntercepted(MethodInfo methodInfo, object[] methodParamsObj)
        {
            bool methodHasAuditAttribute = CheckIfMethodHasAuditAttribute(methodInfo);
            if (!methodHasAuditAttribute)
                return false;

            bool methodHasAuditableParamEqualsTrue = CheckIfMethodHasAuditableParamEqualsTrue(methodInfo, methodParamsObj);
            if (!methodHasAuditableParamEqualsTrue)
                return false;

            bool methodCallersHasAuditAttribute = CheckIfMethodCallersAreBeingIntercepted();
            if (methodCallersHasAuditAttribute)
                return false;

            string methodUniqueId = GetMethodUniqueId(methodInfo);
            _auditingHandler.AwesomeAuditingHandler.AddInterceptedMethod(methodUniqueId);

            return true;
        }

        /// <summary>
        /// Checks if method has Audit attribute.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns><c>true</c> if method has Audit attribute.</returns>
        private bool CheckIfMethodHasAuditAttribute(MethodInfo methodInfo)
        {
            if (GetTheMethodAuditAttribute(methodInfo) == null)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the method Audit attribute.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns>The method Audit attribute.</returns>
        private Audit GetTheMethodAuditAttribute(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<Audit>(true);
        }

        /// <summary>
        /// Checks if method has auditable parameter equals true.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="methodParamsObj">The method parameters object array.</param>
        /// <returns><c>true</c> if method has auditable parameter equals true</returns>
        /// <exception cref="AwesomeAuditing.AwesomeAuditingException">
        /// </exception>
        private bool CheckIfMethodHasAuditableParamEqualsTrue(MethodInfo methodInfo, object[] methodParamsObj)
        {
            IEnumerable<ParameterInfo> auditableParams = methodInfo
                .GetParameters()
                .Where(param => param.GetCustomAttributes<MethodIsAuditableParam>(true).Count() > 0);

            if (auditableParams.Count() > 1)
                throw new AwesomeAuditingException(String.Format(@"Only one parameter with the attribute MethodIsAuditableParam is allowed. 
                    Error in method '{0}'", methodInfo.Name));

            else if (auditableParams.Count() <= 0)
                throw new AwesomeAuditingException(String.Format(@"No parameter with the attribute MethodIsAuditableParam was found. 
                    Error in method '{0}'", methodInfo.Name));

            object auditableParam = methodParamsObj[auditableParams.First().Position];
            bool methodIsAuditable = false;
            if (!Boolean.TryParse(auditableParam.ToString(), out methodIsAuditable))
                throw new AwesomeAuditingException(String.Format(@"The parameter with the attribute MethodIsAuditableParam must be boolean. 
                    Error in method '{0}'", methodInfo.Name));

            return methodIsAuditable;
        }

        /// <summary>
        /// Checks if method callers are being intercepted.
        /// </summary>
        /// <returns><c>true</c> if method callers are being intercepted.</returns>
        private bool CheckIfMethodCallersAreBeingIntercepted()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            StackFrame[] realStackFrames = stackFrames.SubArray(5, stackFrames.Count() - 5);

            List<string> alreadyInterceptedMethods = _auditingHandler.AwesomeAuditingHandler
                .GetAlreadyInterceptedMethods()
                .ToList();

            foreach (StackFrame realStackFrame in realStackFrames)
            {
                MethodBase method = realStackFrame.GetMethod();
                string methodUniqueId = GetMethodUniqueId(method);
                
                if (alreadyInterceptedMethods.Contains(methodUniqueId))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the method unique string identifier based on its fullname and parameters.
        /// </summary>
        /// <param name="method">The method information.</param>
        /// <returns>The unique string identifier.</returns>
        private string GetMethodUniqueId(MethodBase method)
        {
            string methodUniqueId = method.Name;

            if(method.DeclaringType != null)
                methodUniqueId += method.DeclaringType.FullName;

            string paramsString = string.Empty;
            ParameterInfo[] parametersInfo = method.GetParameters();
            foreach (ParameterInfo parameterInfo in parametersInfo)
                paramsString += parameterInfo.Name;

            return methodUniqueId + paramsString;
        }

        /// <summary>
        /// Gets the type of the method return.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns></returns>
        private Type GetMethodReturnType(MethodInfo methodInfo)
        {
            if (CheckIfMethodIsAsync(methodInfo))
                return methodInfo.ReturnType.GetGenericArguments()[0];
            else
                return methodInfo.ReturnType;
        }

        /// <summary>
        /// Checks if method return type inherits from IAuditableEntity interface.
        /// </summary>
        /// <param name="returnType">Type of the intercepted method return.</param>
        /// <returns><c>true</c> if method return type inherits from IAuditableEntity interface.</returns>
        private bool CheckIfMethodReturnTypeInheritAuditEntity(MethodInfo methodInfo)
        {
            Type returnType = GetMethodReturnType(methodInfo);

            if (CheckIfMethodReturnTypeIsEnumerable(returnType))
                return returnType
                    .GetGenericArguments()[0].GetInterfaces().Contains(typeof(IAuditableEntity));

            return returnType.GetInterfaces().Contains(typeof(IAuditableEntity));
        }

        /// <summary>
        /// Checks if method return type is enumerable.
        /// </summary>
        /// <param name="returnType">Type of the intercepted method return.</param>
        /// <returns><c>true</c> if method return type is enumerable.</returns>
        private bool CheckIfMethodReturnTypeIsEnumerable(Type returnType)
        {
            return returnType.IsGenericType &&
                    returnType.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable));
        }

        /// <summary>
        /// Audits the return value.
        /// </summary>
        /// <param name="auditableEntity">The auditable entity.</param>
        /// <param name="methodInfo">The method information.</param>
        private void AuditReturnValue(object auditableEntity, MethodInfo methodInfo) 
        {
            AuditOperation auditOperation = GetAuditOperation(methodInfo);

            if (!CheckIfMethodReturnTypeIsEnumerable(methodInfo.ReturnType))
            {
                Audit(auditableEntity, auditOperation); 
                return;
            }

            IEnumerable auditableEntityList = auditableEntity as IEnumerable;
            foreach (object auditableEntityItem in auditableEntityList)
                Audit(auditableEntityItem, auditOperation);
        }

        /// <summary>
        /// Audits the specified auditable entity.
        /// </summary>
        /// <param name="auditableEntity">The auditable entity.</param>
        /// <param name="operation">The operation.</param>
        private void Audit(object auditableEntity, AuditOperation operation)
        {
            _auditingDomainService.Audit(
                    (IAuditableEntity)auditableEntity,
                    operation,
                    _auditingHandler.AwesomeAuditingHandler.GetAuditingUser());
        }

        /// <summary>
        /// Gets the audit operation.
        /// </summary>
        /// <param name="methodInfo">The method information.</param>
        /// <returns>The AuditOperation enum value.</returns>
        /// <exception cref="AwesomeAuditing.AwesomeAuditingException">When the Audit attribute does not contains the audit operation enum value.</exception>
        /// <seealso cref="AwesomeAuditing.AuditOperation" />
        private AuditOperation GetAuditOperation(MethodInfo methodInfo)
        {
            Audit auditAttr = GetTheMethodAuditAttribute(methodInfo);
            if (auditAttr == null)
                throw new AwesomeAuditingException(String.Format(@"It was not possible to get the auditing operation. 
                    Error in method '{0}'", methodInfo.Name));

            return auditAttr.Operation;
        }
    }
}
