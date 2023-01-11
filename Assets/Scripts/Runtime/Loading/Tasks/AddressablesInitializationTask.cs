using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;
using UnityEngine.AddressableAssets;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    public class AddressablesInitializationTask : BaseLoadingTask
    {
        private readonly Progress _progress = new();
        
        public override IProgressProvider Progress => _progress;
        
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

                _progress.Progress01 = operationHandle.PercentComplete;
            }

            var foundKeysCount = operationHandle.Result.Keys.Count();
            UnityEngine.Debug.Log($"[{nameof(AddressablesInitializationTask)}] Execution finished. Found {foundKeysCount} keys");

            return true;
        }
    }
}