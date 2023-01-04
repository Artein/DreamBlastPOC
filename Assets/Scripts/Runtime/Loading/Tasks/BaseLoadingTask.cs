using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;
using UnityEngine.Assertions;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    public abstract class BaseLoadingTask : ILoadingTask
    {
        private readonly Progress _progress;
        
        public bool IsExecuting { get; private set; }
        public IProgressProvider Progress => _progress;

        protected BaseLoadingTask()
        {
            _progress = new Progress();
            // _progress = new Progress(true, $"{GetType().Name}: ");
        }
        
        public async UniTask<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            Assert.IsFalse(IsExecuting);
            IsExecuting = true;
            var success = await ExecuteAsync_Implementation(cancellationToken);
            IsExecuting = false;

            return success;
        }

        protected abstract UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken);

        protected void SetProgress(float value)
        {
            _progress.Progress01 = value;
        }
    }
}