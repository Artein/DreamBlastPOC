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
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly PreloadAddressableLabelTask _preloadAddressableLabelTask;
        private readonly ProjectInstallerAddressablesLoadingTask _projectInstallerAddressablesLoadingTask;

        public BootstrapExecutor(AssetReferenceScene targetSceneRef, ICancellationTokenProvider lifetimeCTProvider,
            AssetLabelReference preLoadAssetLabel, ProjectInstallerAddressablesLoadingTask projectInstallerAddressablesLoadingTask)
        {
            _lifetimeCTProvider = lifetimeCTProvider;
            _sceneLoadingTask = new SceneLoadingTask(targetSceneRef);
            _preloadAddressableLabelTask = new PreloadAddressableLabelTask(preLoadAssetLabel);
            _projectInstallerAddressablesLoadingTask = projectInstallerAddressablesLoadingTask;
        }
        
        public async void Initialize()
        {
            var addressablesInitializationTask = new AddressablesInitializationTask();
            await addressablesInitializationTask.ExecuteAsync(_lifetimeCTProvider.Token);

            await _projectInstallerAddressablesLoadingTask.ExecuteAsync(_lifetimeCTProvider.Token);
            
            _preloadAddressableLabelTask.Progress.Changed += LogPreloadAddressableLabelProgress;
            
            await _preloadAddressableLabelTask.ExecuteAsync(_lifetimeCTProvider.Token);
            
            _preloadAddressableLabelTask.Progress.Changed -= LogPreloadAddressableLabelProgress;
            _sceneLoadingTask.Progress.Changed += LogSceneLoadingProgress;
            
            await _sceneLoadingTask.ExecuteAsync(_lifetimeCTProvider.Token);
            
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