using System;
using JetBrains.Annotations;

namespace Game.Utils
{
    public interface IDeferredInvocation
    {
        [MustUseReturnValue]
        IDisposable Lock();
    }
}