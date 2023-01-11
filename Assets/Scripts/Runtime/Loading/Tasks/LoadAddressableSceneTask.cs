using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using Game.Utils.Progression;
using JetBrains.Annotations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    [UsedImplicitly]
    public class LoadAddressableSceneTask : BaseLoadingTask
    {
        private readonly AssetReferenceScene _sceneRef;
        private readonly bool _activateOnLoad;
        private readonly LoadSceneMode _loadSceneMode;
        private readonly Progress _progress = new();
        private readonly AddressableScenesStorage _addressableScenesStorage;

        public override IProgressProvider Progress => _progress;

        public LoadAddressableSceneTask(AddressableScenesStorage addressableScenesStorage, AssetReferenceScene sceneRef, LoadSceneMode loadSceneMode, bool activateOnLoad = true)
        {
            _addressableScenesStorage = addressableScenesStorage;
            _loadSceneMode = loadSceneMode;
            _activateOnLoad = activateOnLoad;
            _sceneRef = sceneRef;
        }

        public override string ToString()
        {
            return $"{nameof(LoadAddressableSceneTask)}({_sceneRef.SceneName})";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var operation = _sceneRef.LoadSceneAsync(_loadSceneMode, _activateOnLoad);
            _addressableScenesStorage.AddLoadOperation(_sceneRef, operation);
            SetProgress(ref operation);

            while (!operation.IsDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress(ref operation);
            }

            return true;
        }

        // ideally I would use in-keyword but the warning is shown
        private void SetProgress(ref AsyncOperationHandle<SceneInstance> handle)
        {
            _progress.Progress01 = handle.GetDownloadStatus().Percent * handle.PercentComplete;
        }
    }
}