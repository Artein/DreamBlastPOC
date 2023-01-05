using System;
using JetBrains.Annotations;

namespace Game.Utils.Locking
{
    // TODO: Move to UnityUtils
    public interface ILocker
    {
        bool IsLocked { get; }
        
        [MustUseReturnValue("Release disposable handle to Unlock")]
        IDisposable Lock();
    }
}