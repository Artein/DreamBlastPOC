using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using Game.Utils.Progression;
using UnityEngine;
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
            return nameof(AddressablesInitializationTask);
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var operationHandle = Addressables.InitializeAsync(); // does autorelease operationHandle

            try
            {
                while (!operationHandle.IsDone)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.DelayFrame(1, cancellationToken: cancellationToken);

                    _progress.Progress01 = operationHandle.PercentComplete;
                }
            }
            catch (OperationCanceledException)
            {
                Log(nameof(OperationCanceledException));
                throw;
            }
            finally
            {
                if (operationHandle.TryGetDownloadError(out var errorMessage))
                {
                    Log($"Download error '{errorMessage}'");
                }
                
                var foundKeysCount = operationHandle.Result?.Keys.Count() ?? 0;
                Log($"Execution finished ({operationHandle.Status}). Found {foundKeysCount} keys");
            }

            return true;
        }

        private static void Log(string message)
        {
            Debug.unityLogger.Log(nameof(AddressablesInitializationTask), message);
        }
    }
}