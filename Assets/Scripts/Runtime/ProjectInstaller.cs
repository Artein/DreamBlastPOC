using System.Threading;
using Game.Chips;
using Game.Input;
using Game.Level;
using Game.Loading.Tasks;
using Game.Platform;
using Game.Utils;
using Game.Utils.Addressable;
using NaughtyAttributes;
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
        [SerializeField] private AssetReferenceScene _levelSceneRef;
        [SerializeField] private AssetLabelReference _preLoadAssetLabel;
        [SerializeField, Layer] private int _ignoreRaycastsLayer;
        [SerializeField, Layer] private int _chipLayer;
        
        private CancellationTokenSource _lifetimeCTS = new();

        private void OnValidate()
        {
            _coloredChipsActivationConfigRef.RuntimeKeyIsValid();
            _chipViewsConfigRef.RuntimeKeyIsValid();
            _preLoadAssetLabel.RuntimeKeyIsValid();
            _levelsConfigRef.RuntimeKeyIsValid();
            _levelSceneRef.RuntimeKeyIsValid();
        }

        private void OnDestroy()
        {
            _lifetimeCTS.Cancel();
            _lifetimeCTS.Dispose();
            _lifetimeCTS = null;
        }

        public override void Start()
        {
            var bootstrap = Container.Instantiate<Bootstrap>(new object[]{ _levelSceneRef });
            bootstrap.Execute();
        }

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.Bind<ICancellationTokenProvider>().FromInstance(new CancellationTokenProvider(_lifetimeCTS)).AsSingle();
            Container.Bind<LoadProjectInstallerAddressablesTask>().AsSingle();
            Container.BindInterfacesTo<TargetFPSController>().AsSingle();
            
            BindOptions();
            BindInput();
            BindLayers();
            BindChipsConfigs();
            BindLevels();
            BindCameraRig();

            Container.BindInstance(_levelSceneRef).WithId(InjectionIds.AssetReferenceScene.Level).AsCached().NonLazy();
        }

        private void BindOptions()
        {
            Container.BindInstance(SRDebug.Instance).AsSingle();
            Container.BindInterfacesTo<ProjectOptions>().AsSingle();
            Container.BindInterfacesTo<GameLevelsOptions>().AsSingle();
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

        private void BindLayers()
        {
            Container.BindInstance(_chipLayer).WithId(InjectionIds.Int.ChipsLayer);
            Container.BindInstance(_ignoreRaycastsLayer).WithId(InjectionIds.Int.IgnoreRaycastsLayer);
        }

        private void BindCameraRig()
        {
            var cameraRig = GameObject.FindWithTag("CameraRig");
            Assert.IsNotNull(cameraRig);
            Container.BindInstance(cameraRig.transform).WithId(InjectionIds.Transform.CameraRig);
        }
    }
}