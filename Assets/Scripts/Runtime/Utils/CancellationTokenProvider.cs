using System.Threading;

namespace Game.Utils
{
    public class CancellationTokenProvider : ICancellationTokenProvider
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public CancellationToken Token => _cancellationTokenSource.Token;

        public CancellationTokenProvider(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }
    }
}