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
            var operationHandle = Addressables.InitializeAsync(); // does autorelease operationHandle
            // At first value is 1, but next frame 0, and only then 1 again
            // SetProgress(operationHandle.PercentComplete);
                
            while (!operationHandle.IsDone)
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress(operationHandle.PercentComplete);
            }

            var foundKeysCount = operationHandle.Result.Keys.Count();
            UnityEngine.Debug.Log($"[{nameof(AddressablesInitializationTask)}] Execution finished. Found {foundKeysCount} keys");

            return true;
        }
    }
}