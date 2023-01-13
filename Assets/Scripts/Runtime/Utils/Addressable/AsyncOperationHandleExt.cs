using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public static void Release<T>(this List<AsyncOperationHandle<T>> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var handle = list[i];
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }
        
        public static void Release(this List<AsyncOperationHandle> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var handle = list[i];
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
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

        [MustUseReturnValue]
        public static List<UniTask<T>> ToUniTask<T>(this List<AsyncOperationHandle<T>> list)
        {
            var result = new List<UniTask<T>>(list.Count);
            for (int i = 0; i < list.Count; i += 1)
            {
                result.Add(list[i].ToUniTask());
            }

            return result;
        }
        
        [MustUseReturnValue]
        public static List<UniTask> ToUniTask(this List<AsyncOperationHandle> list)
        {
            var result = new List<UniTask>(list.Count);
            for (int i = 0; i < list.Count; i += 1)
            {
                result.Add(list[i].ToUniTask());
            }

            return result;
        }
    }
}