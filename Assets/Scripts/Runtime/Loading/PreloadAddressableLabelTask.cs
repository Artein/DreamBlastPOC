using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Loading
{
    public class PreloadAddressableLabelTask : BaseLoadingTask
    {
        private readonly AssetLabelReference _assetLabelReference;

        public PreloadAddressableLabelTask(AssetLabelReference assetLabelReference)
        {
            _assetLabelReference = assetLabelReference;
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
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
                SetProgress(downloadDependenciesHandle.GetDownloadStatus().Percent);
                    
                do
                {
                    SetProgress(downloadDependenciesHandle.GetDownloadStatus().Percent);

                    await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                } while (!downloadDependenciesHandle.IsDone);
                    
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