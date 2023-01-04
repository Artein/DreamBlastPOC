using System;
using JetBrains.Annotations;

namespace Game.Utils
{
    public class DeferredInvocation : IDeferredInvocation, IDisposable
    {
        private int _locksCount;
        private bool _isDisposed;
        private IDisposable _actionInvocationHandle;

        public bool IsLocked => _locksCount > 0;
        
        public DeferredInvocation([CanBeNull] Action action, bool lockByDefault = true)
        {
            if (lockByDefault)
            {
                _locksCount = 1;
            }
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

            if (!IsLocked)
            {
                _actionInvocationHandle.Dispose();
                _actionInvocationHandle = null;
            }
        }
    }
}