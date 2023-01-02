using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using Zenject;

namespace Game.Helpers
{
    [UsedImplicitly]
    public class LoadLevelsSceneCommand
    {
        private readonly SceneLoadingTask _sceneLoadingTask;

        public LoadLevelsSceneCommand(
            [Inject(Id = InjectionIds.AssetReferenceScene.Level)] AssetReferenceScene targetSceneRef)
        {
            _sceneLoadingTask = new SceneLoadingTask(targetSceneRef);
        }
        
        public async UniTask ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _sceneLoadingTask.LoadAsync(cancellationToken);
        }
    }
}