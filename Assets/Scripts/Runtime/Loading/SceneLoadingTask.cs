using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Game.Loading
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

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            var operation = _sceneRef.LoadSceneAsync(LoadSceneMode.Single, _activateOnLoad);
            SetProgress(operation.GetDownloadStatus().Percent);

            do
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress(operation.GetDownloadStatus().Percent);
            } while (!operation.IsDone);

            return true;
        }
    }
}