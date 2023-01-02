using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using Progress = Game.Utils.Progress;

namespace Game.Loading
{
    public class AddressablesInitializationTask : ILoadingTask
    {
        private readonly Progress _progress = new();
        
        public bool IsLoading { get; private set; }

        public IProgressProvider Progress => _progress;
        
        public async UniTask<bool> LoadAsync(CancellationToken cancellationToken)
        {
            Assert.IsFalse(IsLoading);
            IsLoading = true;
            {
                var operationHandle = Addressables.InitializeAsync(true);
                _progress.Progress01 = operationHandle.PercentComplete;
                
                do
                {
                    await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                    
                    _progress.Progress01 = operationHandle.PercentComplete;
                } while (!operationHandle.IsDone);
            }
            IsLoading = false;

            return true;
        }
    }
}