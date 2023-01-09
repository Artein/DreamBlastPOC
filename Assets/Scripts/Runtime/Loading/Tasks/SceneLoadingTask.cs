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
    public class SceneLoadingTask : BaseLoadingTask
    {
        private readonly AssetReferenceScene _sceneRef;
        private readonly bool _activateOnLoad;

        public SceneLoadingTask(AssetReferenceScene sceneRef, bool activateOnLoad = true)
        {
            _activateOnLoad = activateOnLoad;
            _sceneRef = sceneRef;
        }
        
        public override string ToString()
        {
            return $"{nameof(SceneLoadingTask)}({_sceneRef.SceneName})";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            // TODO: Use operation to unload scene when will be Additive mode + Loading scene
            var operation = _sceneRef.LoadSceneAsync(LoadSceneMode.Single, _activateOnLoad);
            // releasing a scene is unloading it, so we don't do that
            // using var operationReleaseHandle = operation.ReleaseInScope();
            SetProgress(ref operation);

            while (!operation.IsDone)
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress(ref operation);
            }

            return true;
        }

        // ideally I would use in-keyword but the warning is shown
        private void SetProgress(ref AsyncOperationHandle<SceneInstance> handle)
        {
            SetProgress(handle.GetDownloadStatus().Percent * handle.PercentComplete);
        }
    }
}