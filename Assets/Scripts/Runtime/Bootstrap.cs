using Cysharp.Threading.Tasks;
using Game.Loading;
using Game.Loading.Tasks;
using Game.Utils;
using Game.Utils.Addressable;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Game
{
    [UsedImplicitly]
    public class Bootstrap
    {
        private readonly Loader _loader;
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly LoadAddressableSceneTask _loadTargetSceneTask;
        private readonly LoadProjectInstallerAddressablesTask _loadProjectInstallerAddressablesTask;

        public Bootstrap(AssetReferenceScene targetSceneRef, ICancellationTokenProvider lifetimeCTProvider, Loader loader,
            LoadProjectInstallerAddressablesTask loadProjectInstallerAddressablesTask, AddressableScenesStorage addressableScenesStorage)
        {
            _loader = loader;
            _lifetimeCTProvider = lifetimeCTProvider;
            _loadTargetSceneTask = new LoadAddressableSceneTask(addressableScenesStorage, targetSceneRef, LoadSceneMode.Additive);
            _loadProjectInstallerAddressablesTask = loadProjectInstallerAddressablesTask;
        }

        public void Execute()
        {
            _loader.Reset();
            _loader
                .Enqueue(1, new AddressablesInitializationTask())
                .Enqueue(1, new DownloadAllAddressablesDependenciesTask())
                .Enqueue(1, _loadProjectInstallerAddressablesTask)
                .Enqueue(1, _loadTargetSceneTask)
                .StartAsync(_lifetimeCTProvider.Token)
                .Forget();
        }
    }
}