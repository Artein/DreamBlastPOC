using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;

namespace Game.Loading.Tasks
{
    public interface ILoadingTask
    {
        bool IsExecuting { get; }
        IProgressProvider Progress { get; }
        UniTask<bool> ExecuteAsync(CancellationToken cancellationToken);
    }
}