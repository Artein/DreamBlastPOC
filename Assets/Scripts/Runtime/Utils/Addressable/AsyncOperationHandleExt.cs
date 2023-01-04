using System;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Utils.Addressable
{
    public static class AsyncOperationHandleExt
    {
        [MustUseReturnValue]
        public static IDisposable ReleaseInScope<T>(this AsyncOperationHandle<T> handle)
        {
            return new DisposableAction(() => Addressables.Release(handle));
        }

        [MustUseReturnValue]
        public static IDisposable ReleaseInScope(this AsyncOperationHandle handle)
        {
            return new DisposableAction(() => Addressables.Release(handle));
        }
    }
}