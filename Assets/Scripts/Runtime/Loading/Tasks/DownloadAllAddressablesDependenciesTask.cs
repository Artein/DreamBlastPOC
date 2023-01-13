using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;
using Game.Utils.Addressable;
using Game.Utils.Progression;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityUtils.Extensions;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    public class DownloadAllAddressablesDependenciesTask : BaseLoadingTask
    {
        private readonly Progress _progress = new();
        private readonly List<AsyncOperationHandle<long>> _downloadSizeOperations = new();
        private readonly List<AsyncOperationHandle> _downloadOperations = new();
        private const float AcquireSizeWeight = 0.1f;
        private const float DownloadDependenciesWeight = 1f - AcquireSizeWeight;

        public override IProgressProvider Progress
        {
            get
            {
                var acquireSizeTotalProgress = 0f;
                if (_downloadSizeOperations.Count > 0)
                {
                    for (int i = 0; i < _downloadSizeOperations.Count; i += 1)
                    {
                        acquireSizeTotalProgress += _downloadSizeOperations[i].GetDownloadStatus().Percent;
                    }
                    
                    acquireSizeTotalProgress /= _downloadSizeOperations.Count;
                }

                var downloadTotalProgress = 0f;
                if (_downloadOperations.Count > 0)
                {
                    for (int i = 0; i < _downloadOperations.Count; i += 1)
                    {
                        downloadTotalProgress += _downloadOperations[i].GetDownloadStatus().Percent;
                    }

                    downloadTotalProgress /= _downloadOperations.Count;
                }

                Assert.IsTrue(AcquireSizeWeight + DownloadDependenciesWeight == 1f);
                _progress.Progress01 = acquireSizeTotalProgress * AcquireSizeWeight + downloadTotalProgress * DownloadDependenciesWeight;
                return _progress;
            }
        }

        public override string ToString()
        {
            return nameof(DownloadAllAddressablesDependenciesTask);
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            _progress.Reset();
            var resourceLocators = Addressables.ResourceLocators.ToList();
            _downloadSizeOperations.Clear();
            _downloadSizeOperations.EnsureSize(resourceLocators.Count);
            _downloadOperations.Clear();
            
            try
            {
                for (int i = 0; i < resourceLocators.Count; i += 1)
                {
                    var resourceLocator = resourceLocators[i];
                    _downloadSizeOperations[i] = Addressables.GetDownloadSizeAsync(resourceLocator.Keys);
                }
            
                var downloadSizeBytesList = await UniTask.WhenAll(_downloadSizeOperations.ToUniTask()).AttachExternalCancellation(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var totalBytes = downloadSizeBytesList.Sum();
                Log($"Acquire sizes finished. To download ~{BytesConversionUtils.ToHumanizedString(totalBytes)}({totalBytes} bytes)");

                for (int i = 0; i < resourceLocators.Count; i += 1)
                {
                    var downloadSizeBytes = downloadSizeBytesList[i];
                    if (downloadSizeBytes > 0)
                    {
                        var resourceLocator = resourceLocators[i];
                        foreach (var resourceKey in resourceLocator.Keys)
                        {
                            var downloadOperation = Addressables.DownloadDependenciesAsync(resourceKey);
                            _downloadOperations.Add(downloadOperation);
                        }
                    }
                }

                await UniTask.WhenAll(_downloadOperations.ToUniTask()).AttachExternalCancellation(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                _downloadSizeOperations.Release();
                _downloadSizeOperations.Clear();
                _downloadOperations.Release();
                _downloadOperations.Clear();
            }

            return true;
        }

        private static void Log(string message)
        {
            Debug.unityLogger.Log(nameof(DownloadAllAddressablesDependenciesTask), message);
        }
    }
}