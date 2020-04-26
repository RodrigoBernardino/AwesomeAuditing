
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeAuditing
{
    /// <summary>
    /// Abstract AwesomeAuditing user handler class.
    /// </summary>
    public abstract class AwesomeAuditingHandler
    {
        private readonly Timer _timer;
        private readonly object _lock = new object();
        private bool _isAlreadyExecuting = false;

        private static ConcurrentDictionary<Thread, ConcurrentBag<string>> _auditingInteceptedMethodsByThread;

        public AwesomeAuditingHandler()
        {
            _timer = new Timer(new TimerCallback(c => TryClearThreadDictionary()), null, 0, 300);
        }

        /// <summary>
        /// Custom method that gets the auditing user.
        /// </summary>
        /// <returns>The string value that represent the auditing action user.</returns>
        public abstract string GetAuditingUser();

        /// <summary>
        /// Custom method that gets the already intercepted methods.
        /// </summary>
        /// <returns>The string value that represent the auditing action user.</returns>
        public virtual IEnumerable<string> GetAlreadyInterceptedMethods()
        {
            if (!ThreadIsAlreadyAdded())
                StartAuditingHandler();

            ConcurrentBag<string> auditingInteceptedMethods =
                _auditingInteceptedMethodsByThread[Thread.CurrentThread];

            return auditingInteceptedMethods;
        }

        /// <summary>
        /// Custom method that adds a new method in the intercepted method list.
        /// </summary>
        /// <param name="methodUniqueId">The method unique identifier.</param>
        public virtual void AddInterceptedMethod(string interceptedMethodUniqueId)
        {
            if (!ThreadIsAlreadyAdded())
                StartAuditingHandler();

            ConcurrentBag<string> auditingInteceptedMethods =
                _auditingInteceptedMethodsByThread[Thread.CurrentThread];

            auditingInteceptedMethods.Add(interceptedMethodUniqueId);
        }

        public void StartAuditingHandler()
        {
            if (_auditingInteceptedMethodsByThread == null)
                _auditingInteceptedMethodsByThread = new ConcurrentDictionary<Thread, ConcurrentBag<string>>();

            if (!ThreadIsAlreadyAdded())
            {
                bool addSuccessfully = _auditingInteceptedMethodsByThread.TryAdd(Thread.CurrentThread, new ConcurrentBag<string>());
                if (!addSuccessfully)
                    throw new AwesomeAuditingException("It was not possible to start auditing handler context.");
            }
        }

        private static bool ThreadIsAlreadyAdded()
        {
            if (_auditingInteceptedMethodsByThread == null)
                _auditingInteceptedMethodsByThread = new ConcurrentDictionary<Thread, ConcurrentBag<string>>();

            return _auditingInteceptedMethodsByThread.ContainsKey(Thread.CurrentThread);
        }

        private void TryClearThreadDictionary()
        {
            if (!_isAlreadyExecuting)
            {
                lock (_lock)
                {
                    if (!_isAlreadyExecuting)
                    {
                        ClearThreadDictionary();
                    }
                }
            }
        }

        private void ClearThreadDictionary()
        {
            _isAlreadyExecuting = true;

            if (_auditingInteceptedMethodsByThread == null)
            {
                _isAlreadyExecuting = false;
                return;
            }

            Parallel.ForEach(_auditingInteceptedMethodsByThread.Keys.ToList(), thread =>
            {
                if (!thread.IsAlive)
                {
                    ConcurrentBag<string> currentInterceptedMethods;
                    bool removeSuccessfully = _auditingInteceptedMethodsByThread.TryRemove(thread, out currentInterceptedMethods);
                    if (!removeSuccessfully)
                        throw new AwesomeAuditingException("It was not possible to end auditing handler context.");
                }
            });

            _isAlreadyExecuting = false;
        }
    }
}
