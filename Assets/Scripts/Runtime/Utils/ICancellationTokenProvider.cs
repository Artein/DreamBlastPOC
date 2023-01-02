using System.Threading;

namespace Game.Utils
{
    public interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}