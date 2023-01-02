using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;

namespace Game.Loading
{
    public interface ILoadingTask
    {
        bool IsLoading { get; }
        IProgressProvider Progress { get; }
        UniTask<bool> LoadAsync(CancellationToken cancellationToken);

        public delegate void ProgressO1ChangedHandler(float value, float prevValue);
    }
}