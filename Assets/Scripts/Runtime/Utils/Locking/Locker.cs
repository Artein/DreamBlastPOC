using System;
using UnityEngine.Assertions;
using UnityUtils.Invocation;

namespace Game.Utils.Locking
{
    // TODO: Move to UnityUtils
    // TODO: Add Reset() to support multiple usage and decrease GC (Same for DisposableAction)
    public class Locker : ILocker, IDisposable
    {
        private int _locksCount;
        private bool _isDisposed;
    
        public bool IsLocked => _locksCount > 0;
        
        public Locker(bool locked = true)
        {
            if (locked)
            {
                _locksCount = 1;
            }
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
            Assert.IsTrue(_locksCount >= 1);
            _locksCount -= 1;
        }
    }
}