using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Game.Loading.Tasks
{
    public class AddressablesInitializationTask : BaseLoadingTask
    {
        public override string ToString()
        {
            return $"{nameof(AddressablesInitializationTask)}";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var operationHandle = Addressables.InitializeAsync(); // does autorelease operationHandle
                
            while (!operationHandle.IsDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress01(operationHandle.PercentComplete);
            }

            var foundKeysCount = operationHandle.Result.Keys.Count();
            UnityEngine.Debug.Log($"[{nameof(AddressablesInitializationTask)}] Execution finished. Found {foundKeysCount} keys");

            return true;
        }
    }
}