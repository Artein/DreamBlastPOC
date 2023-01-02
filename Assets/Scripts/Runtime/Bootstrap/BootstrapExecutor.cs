using Cysharp.Threading.Tasks;
using Game.Level;
using Game.Loading;
using Game.Utils;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Bootstrap
{
    [UsedImplicitly]
    public class BootstrapExecutor : IInitializable
    {
        private readonly SceneLoadingTask _sceneLoadingTask;
        private readonly LevelsController _levelsController;
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly PreloadAddressableLabelTask _preloadAddressableLabelTask;

        public BootstrapExecutor(AssetReferenceScene targetSceneRef, ICancellationTokenProvider lifetimeCTProvider, LevelsController levelsController,
            AssetLabelReference preLoadAssetLabel)
        {
            _levelsController = levelsController;
            _lifetimeCTProvider = lifetimeCTProvider;
            _sceneLoadingTask = new SceneLoadingTask(targetSceneRef);
            _preloadAddressableLabelTask = new PreloadAddressableLabelTask(preLoadAssetLabel);
        }
        
        public async void Initialize()
        {
            var addressablesInitializationTask = new AddressablesInitializationTask();
            await addressablesInitializationTask.LoadAsync(_lifetimeCTProvider.Token);
            
            // TODO: Hack. Ideally I would like to receive Initialize when all AddressableInject were resolved.
            // Right now LevelsController is not initialized but we starting to load next scene
            await UniTask.WaitUntil(() => _levelsController.Initialized, cancellationToken: _lifetimeCTProvider.Token);
            
            _preloadAddressableLabelTask.Progress.Changed += LogPreloadAddressableLabelProgress;
            
            await _preloadAddressableLabelTask.LoadAsync(_lifetimeCTProvider.Token);
            
            _preloadAddressableLabelTask.Progress.Changed -= LogPreloadAddressableLabelProgress;
            _sceneLoadingTask.Progress.Changed += LogSceneLoadingProgress;
            
            await _sceneLoadingTask.LoadAsync(_lifetimeCTProvider.Token);
            
            _sceneLoadingTask.Progress.Changed -= LogSceneLoadingProgress;
        }

        private static void LogSceneLoadingProgress(float curr, float prev)
        {
            UnityEngine.Debug.Log($"Scene loading: {curr * 100f}%");
        }
        
        private static void LogPreloadAddressableLabelProgress(float curr, float prev)
        {
            UnityEngine.Debug.Log($"Preload addressable label: {curr * 100f}%");
        }
    }
}