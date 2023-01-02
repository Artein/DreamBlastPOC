using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Utils.Addressable
{
    public static class AsyncOperationHandleExt
    {
        public static IDisposable ReleaseInScope<T>(this AsyncOperationHandle<T> handle)
        {
            return new DisposableAction(() => Addressables.Release(handle));
        }

        public static IDisposable ReleaseInScope(this AsyncOperationHandle handle)
        {
            return new DisposableAction(() => Addressables.Release(handle));
        }
    }
}