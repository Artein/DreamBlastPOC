using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Loading;
using Game.Loading.Tasks;
using Game.Utils;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Game
{
    [UsedImplicitly]
    public class Bootstrap
    {
        private readonly Loader _loader;
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly LoadAddressableSceneTask _loadTargetSceneTask;
        private readonly PreloadAddressableLabelTask _preloadAddressableLabelTask;
        private readonly LoadProjectInstallerAddressablesTask _loadProjectInstallerAddressablesTask;

        public Bootstrap(AssetReferenceScene targetSceneRef, ICancellationTokenProvider lifetimeCTProvider, Loader loader,
            AssetLabelReference preLoadAssetLabel, LoadProjectInstallerAddressablesTask loadProjectInstallerAddressablesTask,
            AddressableScenesStorage addressableScenesStorage)
        {
            _loader = loader;
            _lifetimeCTProvider = lifetimeCTProvider;
            _loadTargetSceneTask = new LoadAddressableSceneTask(addressableScenesStorage, targetSceneRef, LoadSceneMode.Additive);
            _preloadAddressableLabelTask = new PreloadAddressableLabelTask(preLoadAssetLabel);
            _loadProjectInstallerAddressablesTask = loadProjectInstallerAddressablesTask;
        }

        public void Execute()
        {
            _loader.Reset();
            _loader
                .Enqueue(10, new AddressablesInitializationTask())
                .Enqueue(20,
                    new CompositeLoadingTask(new List<WeightedLoadingTask>
                    {
                        new(_loadProjectInstallerAddressablesTask),
                        new(_preloadAddressableLabelTask),
                    }))
                .Enqueue(70, _loadTargetSceneTask);

            _loader.StartAsync(_lifetimeCTProvider.Token).Forget();
        }
    }
}