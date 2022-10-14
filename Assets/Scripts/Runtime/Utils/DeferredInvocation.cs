using System;

namespace Game.Utils
{
    public class DeferredInvocation : IDeferredInvocation, IDisposable
    {
        private int _locksCount;
        private bool _isDisposed;
        private IDisposable _actionInvocationHandle;
        
        public DeferredInvocation(Action action)
        {
            _locksCount = 1;
            _actionInvocationHandle = new DisposableAction(action);
        }

        public IDisposable Lock()
        {
            _locksCount += 1;
            return new DisposableAction(Unlock);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                Unlock();
            }
        }

        private void Unlock()
        {
            _locksCount -= 1;

            if (_locksCount == 0)
            {
                _actionInvocationHandle.Dispose();
                _actionInvocationHandle = null;
            }
        }
    }
}