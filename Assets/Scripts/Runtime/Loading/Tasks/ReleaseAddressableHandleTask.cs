using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    public class ReleaseAddressableHandleTask<TObject> : BaseLoadingTask
    {
        private readonly Progress _progress = new();
        private readonly AsyncOperationHandle<TObject> _handle;

        public override IProgressProvider Progress => _progress;

        public ReleaseAddressableHandleTask(AsyncOperationHandle<TObject> handle)
        {
            _handle = handle;
        }
        
        protected override UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            Addressables.Release(_handle);
            return new UniTask<bool>(true);
        }
    }
    
    public class ReleaseAddressableHandleTask : BaseLoadingTask
    {
        private readonly Progress _progress = new();
        private readonly AsyncOperationHandle _handle;

        public override IProgressProvider Progress => _progress;

        public ReleaseAddressableHandleTask(AsyncOperationHandle handle)
        {
            _handle = handle;
        }
        
        protected override UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            Addressables.Release(_handle);
            return new UniTask<bool>(true);
        }
    }
}