using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine.Assertions;
using Progress = Game.Utils.Progress;

namespace Game.Loading
{
    public abstract class BaseLoadingTask : ILoadingTask
    {
        private readonly Progress _progress = new();
        
        public bool IsLoading { get; private set; }
        public IProgressProvider Progress => _progress;
        
        public async UniTask<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            Assert.IsFalse(IsLoading);
            IsLoading = true;
            var success = await ExecuteAsync_Implementation(cancellationToken);
            IsLoading = false;

            return success;
        }

        protected abstract UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken);

        protected void SetProgress(float value)
        {
            _progress.Progress01 = value;
        }
    }
}