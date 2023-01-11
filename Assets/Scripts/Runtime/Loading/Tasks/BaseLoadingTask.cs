using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;
using UnityEngine.Assertions;

namespace Game.Loading.Tasks
{
    public abstract class BaseLoadingTask : ILoadingTask
    {
        public bool IsExecuting { get; private set; }
        public abstract IProgressProvider Progress { get; }

        public async UniTask<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assert.IsFalse(IsExecuting);
            IsExecuting = true;
            var success = await ExecuteAsync_Implementation(cancellationToken);
            IsExecuting = false;

            return success;
        }

        protected abstract UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken);
    }
}