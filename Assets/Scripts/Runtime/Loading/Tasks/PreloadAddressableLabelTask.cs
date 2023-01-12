using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using Game.Utils.Progression;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    public class PreloadAddressableLabelTask : BaseLoadingTask
    {
        private readonly AssetLabelReference _assetLabelReference;
        private readonly Progress _progress = new();

        public override IProgressProvider Progress => _progress;

        public PreloadAddressableLabelTask(AssetLabelReference assetLabelReference)
        {
            _assetLabelReference = assetLabelReference;
        }

        public override string ToString()
        {
            return $"{nameof(PreloadAddressableLabelTask)}({_assetLabelReference.labelString})";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var getDownloadSizeHandle = Addressables.GetDownloadSizeAsync(_assetLabelReference);
            using var getDownloadSizeReleaseHandle = getDownloadSizeHandle.ReleaseInScope();
                
            var bytesToDownload = await getDownloadSizeHandle;

            if (LogIfFailed(getDownloadSizeHandle))
            {
                return false;
            }
                
            if (bytesToDownload > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var downloadDependenciesHandle = Addressables.DownloadDependenciesAsync(_assetLabelReference);
                using var downloadDependenciesReleaseHandle = downloadDependenciesHandle.ReleaseInScope();
                _progress.Progress01 = downloadDependenciesHandle.GetDownloadStatus().Percent;
                    
                while (!downloadDependenciesHandle.IsDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                    
                    _progress.Progress01 = downloadDependenciesHandle.GetDownloadStatus().Percent;
                }
                
                if (LogIfFailed(downloadDependenciesHandle))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool LogIfFailed(AsyncOperationHandle operationHandle)
        {
            if (operationHandle.Status == AsyncOperationStatus.Failed)
            {
                if (operationHandle.TryGetDownloadError(out var errorMessage))
                {
                    Debug.unityLogger.LogError(nameof(PreloadAddressableLabelTask), errorMessage);
                }
                else
                {
                    Debug.unityLogger.LogException(operationHandle.OperationException);
                }
                return true;
            }

            return false;
        }
    }
}