using System;

namespace NPatterns
{
    public class Disposer : IDisposable
    {
        private readonly Action _callOnDispose;

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
