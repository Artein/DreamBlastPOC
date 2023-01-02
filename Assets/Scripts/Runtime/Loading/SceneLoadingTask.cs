using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Progress = Game.Utils.Progress;

namespace Game.Loading
{
    [UsedImplicitly]
    public class SceneLoadingTask : ILoadingTask
    {
        private readonly AssetReferenceScene _sceneRef;
        private readonly bool _activateOnLoad;
        private readonly Progress _progress = new();

        public IProgressProvider Progress => _progress;
        
        public bool IsLoading { get; private set; }

        public SceneLoadingTask(AssetReferenceScene sceneRef, bool activateOnLoad = true)
        {
            _activateOnLoad = activateOnLoad;
            _sceneRef = sceneRef;
        }
        
        public async UniTask<bool> LoadAsync(CancellationToken cancellationToken)
        {
            Assert.IsFalse(IsLoading);
            IsLoading = true;
            {
                var operation = _sceneRef.LoadSceneAsync(LoadSceneMode.Single, _activateOnLoad);

                do
                {
                    var status = operation.GetDownloadStatus();
                    _progress.Progress01 = status.Percent;

                    await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                } while (!operation.IsDone);
            }
            IsLoading = false;

            return true;
        }
    }
}