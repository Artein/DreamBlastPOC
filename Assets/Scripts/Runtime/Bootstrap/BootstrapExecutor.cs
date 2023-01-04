using Cysharp.Threading.Tasks;
using Game.Loading;
using Game.Loading.Tasks;
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
        private readonly Loader _loader;
        private readonly SceneLoadingTask _sceneLoadingTask;
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly PreloadAddressableLabelTask _preloadAddressableLabelTask;
        private readonly ProjectInstallerAddressablesLoadingTask _projectInstallerAddressablesLoadingTask;

        public BootstrapExecutor(AssetReferenceScene targetSceneRef, ICancellationTokenProvider lifetimeCTProvider, Loader loader,
            AssetLabelReference preLoadAssetLabel, ProjectInstallerAddressablesLoadingTask projectInstallerAddressablesLoadingTask)
        {
            _loader = loader;
            _lifetimeCTProvider = lifetimeCTProvider;
            _sceneLoadingTask = new SceneLoadingTask(targetSceneRef);
            _preloadAddressableLabelTask = new PreloadAddressableLabelTask(preLoadAssetLabel);
            _projectInstallerAddressablesLoadingTask = projectInstallerAddressablesLoadingTask;
        }
        
        public void Initialize()
        {
            _loader.Reset();
            _loader.Enqueue(10, new AddressablesInitializationTask())
                .Enqueue(10, _projectInstallerAddressablesLoadingTask) // TODO: start in parallel with _preloadAddressableLabelTask
                .Enqueue(10, _preloadAddressableLabelTask)
                .Enqueue(70, _sceneLoadingTask);
            
            _loader.StartAsync(_lifetimeCTProvider.Token).Forget();
        }
    }
}