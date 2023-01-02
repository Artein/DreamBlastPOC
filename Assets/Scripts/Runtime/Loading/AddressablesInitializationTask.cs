using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Game.Loading
{
    public class AddressablesInitializationTask : BaseLoadingTask
    {
        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            var operationHandle = Addressables.InitializeAsync(true);
            SetProgress(operationHandle.PercentComplete);
                
            do
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress(operationHandle.PercentComplete);
            } while (!operationHandle.IsDone);

            return true;
        }
    }
}