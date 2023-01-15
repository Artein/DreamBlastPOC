using System.Threading;
using Eflatun.SceneReference;
using Game.Chips;
using Game.Input;
using Game.Level;
using Game.Loading.Tasks;
using Game.Platform;
using Game.Utils;
using Game.Utils.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [SerializeField] private AssetReferenceT<ChipViewsConfig> _chipViewsConfigRef;
        [SerializeField] private AssetReferenceT<ColoredChipsActivationConfig> _coloredChipsActivationConfigRef;
        [SerializeField] private AssetReferenceT<LevelsConfig> _levelsConfigRef;
        [SerializeField] private SceneReference _coreSceneRef;
        [SerializeField] private AssetReferenceScene _levelSceneRef;
        [SerializeField] private AssetLabelReference _preLoadAssetLabel;
        
        private CancellationTokenSource _lifetimeCTS = new();

        private void OnValidate()
        {
            _coloredChipsActivationConfigRef.RuntimeKeyIsValid();
            _chipViewsConfigRef.RuntimeKeyIsValid();
            _preLoadAssetLabel.RuntimeKeyIsValid();
            _levelsConfigRef.RuntimeKeyIsValid();
            _levelSceneRef.RuntimeKeyIsValid();
            Assert.IsTrue(_coreSceneRef.HasValue);
        }

        private void OnDestroy()
        {
            _lifetimeCTS.Cancel();
            _lifetimeCTS.Dispose();
            _lifetimeCTS = null;
        }

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.Bind<ICancellationTokenProvider>().FromInstance(new CancellationTokenProvider(_lifetimeCTS)).AsSingle();
            Container.Bind<LoadProjectInstallerAddressablesTask>().AsSingle();
            Container.BindInterfacesTo<TargetFPSController>().AsSingle();
            
            BindOptions();
            BindInput();
            BindChipsConfigs();
            BindLevels();

            Container.BindInstance(_coreSceneRef).WithId(InjectionIds.SceneReference.Core).AsCached().NonLazy();
            Container.BindInstance(_levelSceneRef).WithId(InjectionIds.AssetReferenceScene.Level).AsCached().NonLazy();
        }

        private void BindOptions()
        {
            Container.BindInstance(SRDebug.Instance).AsSingle();
            Container.BindInterfacesTo<ProjectOptions>().AsSingle();
        }

        private void BindChipsConfigs()
        {
            Container.BindAsync<ChipViewsConfig>().FromAssetReferenceT(_chipViewsConfigRef).AsCached();
            Container.BindAsync<ColoredChipsActivationConfig>().FromAssetReferenceT(_coloredChipsActivationConfigRef).AsCached();
        }

        private void BindInput()
        {
            Container.BindInterfacesAndSelfTo<TouchInputNotifier>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameObjectInputNotifier>().AsSingle();
        }

        private void BindLevels()
        {
            Container.BindInterfacesAndSelfTo<LevelsController>().AsSingle();
            Container.BindAsync<LevelsConfig>().FromAssetReferenceT(_levelsConfigRef).AsCached();
        }
    }
}