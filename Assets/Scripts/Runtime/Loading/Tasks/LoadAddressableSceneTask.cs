using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.Loading.Tasks
{
    [UsedImplicitly]
    public class LoadAddressableSceneTask : BaseLoadingTask
    {
        private readonly AssetReferenceScene _sceneRef;
        private readonly bool _activateOnLoad;
        private readonly LoadSceneMode _loadSceneMode;

        public LoadAddressableSceneTask(AssetReferenceScene sceneRef, LoadSceneMode loadSceneMode, bool activateOnLoad = true)
        {
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
            // releasing a scene is unloading it, so we don't do that
            // using var operationReleaseHandle = operation.ReleaseInScope();
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
            SetProgress01(handle.GetDownloadStatus().Percent * handle.PercentComplete);
        }
    }
}