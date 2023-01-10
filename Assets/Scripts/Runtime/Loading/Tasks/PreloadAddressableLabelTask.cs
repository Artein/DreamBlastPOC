using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Loading.Tasks
{
    public class PreloadAddressableLabelTask : BaseLoadingTask
    {
        private readonly AssetLabelReference _assetLabelReference;

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
                
            await getDownloadSizeHandle;

            if (getDownloadSizeHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogException(getDownloadSizeHandle.OperationException);
                return false;
            }
                
            if (getDownloadSizeHandle.Result > 0)
            {
                var downloadDependenciesHandle = Addressables.DownloadDependenciesAsync(_assetLabelReference);
                using var downloadDependenciesReleaseHandle = downloadDependenciesHandle.ReleaseInScope();
                SetProgress01(downloadDependenciesHandle.GetDownloadStatus().Percent);
                    
                while (!downloadDependenciesHandle.IsDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                    
                    SetProgress01(downloadDependenciesHandle.GetDownloadStatus().Percent);
                }
                    
                if (downloadDependenciesHandle.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogException(downloadDependenciesHandle.OperationException);
                    return false;
                }
            }

            return true;
        }
    }
}