using System;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;
using UnityUtils.Invocation;

namespace Game.Utils.Addressable
{
    public static class AsyncOperationHandleExt
    {
        [MustUseReturnValue]
        public static IDisposable ReleaseInScope<T>(this AsyncOperationHandle<T> handle)
        {
            return ((AsyncOperationHandle)handle).ReleaseInScope();
        }

        [MustUseReturnValue]
        public static IDisposable ReleaseInScope(this AsyncOperationHandle handle)
        {
            return new DisposableAction(() => Addressables.Release(handle));
        }

        [MustUseReturnValue]
        public static bool TryGetDownloadError<T>(this AsyncOperationHandle<T> handle, out string error)
        {
            return ((AsyncOperationHandle)handle).TryGetDownloadError(out error);
        }
        
        [MustUseReturnValue]
        public static bool TryGetDownloadError(this AsyncOperationHandle handle, out string error)
        {
            if (handle.Status != AsyncOperationStatus.Failed)
            {
                error = null;
                return false;
            }

            Exception e = handle.OperationException;
            while (e != null)
            {
                if (e is RemoteProviderException remoteException)
                {
                    error = remoteException.WebRequestResult.Error;
                    return true;
                }
                
                e = e.InnerException;
            }
            
            error = null;
            return false;
        }
    }
}