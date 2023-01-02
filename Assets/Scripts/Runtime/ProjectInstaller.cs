using System.Threading;
using Game.Chips;
using Game.Helpers;
using Game.Input;
using Game.Level;
using Game.Loading;
using Game.Platform;
using Game.Utils;
using Game.Utils.Addressable;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [SerializeField] private ChipViewsConfig _chipViewsConfig;
        [SerializeField] private ColoredChipsActivationConfig _coloredChipsActivationConfig;
        [SerializeField] private LevelsConfig.AssetRef _levelsConfigRef;
        [SerializeField] private AssetReferenceScene _levelSceneRef;
        [SerializeField, Layer] private int _ignoreRaycastsLayer;
        [SerializeField, Layer] private int _chipLayer;
        
        private CancellationTokenSource _lifetimeCTS = new();

        private void OnDestroy()
        {
            _lifetimeCTS.Cancel();
            _lifetimeCTS.Dispose();
            _lifetimeCTS = null;
        }
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            ProjectLoadingInstaller.Install(Container);
            
            Container.Bind<ICancellationTokenProvider>().FromInstance(new CancellationTokenProvider(_lifetimeCTS)).AsSingle();
            Container.BindInterfacesTo<TargetFPSController>().AsSingle();
            
            BindOptions();
            BindInput();
            BindLayers();
            BindChipsConfigs();
            BindLevels();

            Container.BindInstance(_levelSceneRef).WithId(InjectionIds.AssetReferenceScene.Level).AsSingle().NonLazy();
            Container.Bind<LoadLevelsSceneCommand>().AsSingle();
        }

        private void BindOptions()
        {
            Container.BindInstance(SRDebug.Instance).AsSingle();
            Container.BindInterfacesTo<GameLevelsOptions>().AsSingle();
        }

        private void BindChipsConfigs()
        {
            Container.BindInstance(_chipViewsConfig).AsSingle();
            Container.BindInstance(_coloredChipsActivationConfig).AsSingle();
        }

        private void BindInput()
        {
            Container.BindInterfacesAndSelfTo<TouchInputNotifier>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameObjectInputNotifier>().AsSingle();
        }

        private void BindLevels()
        {
            Container.BindInterfacesAndSelfTo<LevelsController>().AsSingle();
            Container.BindAsync<LevelsConfig>().FromAssetReferenceT(_levelsConfigRef).AsSingle();
        }

        private void BindLayers()
        {
            Container.BindInstance(_chipLayer).WithId(InjectionIds.Int.ChipsLayer);
            Container.BindInstance(_ignoreRaycastsLayer).WithId(InjectionIds.Int.IgnoreRaycastsLayer);
        }
    }
}