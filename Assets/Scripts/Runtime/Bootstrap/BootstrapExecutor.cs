using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Game.Loading;
using Game.Loading.Tasks;
using Game.Utils;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Bootstrap
{
    [UsedImplicitly]
    public class BootstrapExecutor : IInitializable
    {
        private readonly Loader _loader;
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly LoadAddressableSceneTask _loadTargetSceneTask;
        private readonly UnloadBuiltInSceneTask _unloadBootstrapSceneTask;
        private readonly PreloadAddressableLabelTask _preloadAddressableLabelTask;
        private readonly LoadProjectInstallerAddressablesTask _loadProjectInstallerAddressablesTask;
        private readonly List<GameObject> _disableGameObjectsOnLoading;

        public BootstrapExecutor(AssetReferenceScene targetSceneRef, ICancellationTokenProvider lifetimeCTProvider, Loader loader,
            AssetLabelReference preLoadAssetLabel, LoadProjectInstallerAddressablesTask loadProjectInstallerAddressablesTask,
            [Inject(Id = InjectionIds.SceneReference.Bootstrap)] SceneReference bootstrapSceneRef,
            List<GameObject> disableGameObjectsOnLoading)
        {
            _loader = loader;
            _lifetimeCTProvider = lifetimeCTProvider;
            _loadTargetSceneTask = new LoadAddressableSceneTask(targetSceneRef, LoadSceneMode.Additive);
            _unloadBootstrapSceneTask = new UnloadBuiltInSceneTask(bootstrapSceneRef);
            _preloadAddressableLabelTask = new PreloadAddressableLabelTask(preLoadAssetLabel);
            _loadProjectInstallerAddressablesTask = loadProjectInstallerAddressablesTask;
            _disableGameObjectsOnLoading = disableGameObjectsOnLoading;
        }

        public void Initialize()
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
                .Enqueue(70, _loadTargetSceneTask)
                .Enqueue(10, _unloadBootstrapSceneTask);
            
            DisableComponentsOnLoading();

            _loader.StartAsync(_lifetimeCTProvider.Token).Forget();
        }

        private void DisableComponentsOnLoading()
        {
            for (int i = 0; i < _disableGameObjectsOnLoading.Count; i += 1)
            {
                var gameObject = _disableGameObjectsOnLoading[i];
                gameObject.SetActive(false);
            }
        }
    }
}