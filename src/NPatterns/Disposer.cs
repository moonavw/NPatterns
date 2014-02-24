using System;

namespace NPatterns
{
    /// <summary>
    /// Invokes the registered callback on disposed
    /// </summary>
    public class Disposer : IDisposable
    {
        private readonly Action _callOnDispose;

        /// <summary>
        /// Instantiate a Disposer
        /// </summary>
        /// <param name="callOnDispose">callback to invoke on disposed</param>
        public Disposer(Action callOnDispose)
        {
            _callOnDispose = callOnDispose;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _callOnDispose();
        }

        #endregion
    }
}