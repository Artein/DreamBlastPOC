using Cysharp.Threading.Tasks;
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
            _lifetimeCTProvider.Token.ThrowIfCancellationRequested();

            var projectInstallerAddressablesTask = _projectInstallerAddressablesLoadingTask.ExecuteAsync(_lifetimeCTProvider.Token);
            var preloadAddressableLabelTask = _preloadAddressableLabelTask.ExecuteAsync(_lifetimeCTProvider.Token);
            await UniTask.WhenAll(projectInstallerAddressablesTask, preloadAddressableLabelTask);
            _lifetimeCTProvider.Token.ThrowIfCancellationRequested();
            
            await _sceneLoadingTask.ExecuteAsync(_lifetimeCTProvider.Token);
        }
    }
}