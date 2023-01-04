using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using UnityEngine.AddressableAssets;

namespace Game.Loading
{
    public class AddressablesInitializationTask : BaseLoadingTask
    {
        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            var operationHandle = Addressables.InitializeAsync(true);
            using var operationReleaseHandle = operationHandle.ReleaseInScope();
            SetProgress(operationHandle.PercentComplete);
                
            while (!operationHandle.IsDone)
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress(operationHandle.PercentComplete);
            }

            return true;
        }
    }
}