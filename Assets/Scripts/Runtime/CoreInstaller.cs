using Game.Chips;
using Game.Level;
using Game.Loading.Tasks;
using Game.Utils.Addressable;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using Zenject;

namespace Game
{
    public class CoreInstaller : MonoInstaller<CoreInstaller>
    {
        [SerializeField] private Transform _cameraRigTransform;
        [SerializeField] private AssetReferenceT<LevelsConfig> _levelsConfigRef;
        [SerializeField] private AssetReferenceT<ChipViewsConfig> _chipViewsConfigRef;
        [SerializeField] private AssetReferenceT<ColoredChipsActivationConfig> _coloredChipsActivationConfigRef;
        [SerializeField, Layer] private int _ignoreRaycastsLayer;
        [SerializeField, Layer] private int _chipLayer;
        
        [Inject(Id = InjectionIds.AssetReferenceScene.Level)] private AssetReferenceScene _levelSceneRef;
        
        private void OnValidate()
        {
            Assert.IsNotNull(_cameraRigTransform, "_cameraRigTransform != null");
            _coloredChipsActivationConfigRef.RuntimeKeyIsValid();
            _chipViewsConfigRef.RuntimeKeyIsValid();
            _levelsConfigRef.RuntimeKeyIsValid();
        }

        public override void Start()
        {
            var bootstrap = Container.Instantiate<Bootstrap>(new object[]{ _levelSceneRef });
            bootstrap.Execute();
        }
        
        public override void InstallBindings()
        {
            Container.Bind<LoadProjectInstallerAddressablesTask>().AsSingle();
            
            BindCameraRig();
            BindLayers();
            BindOptions();
            BindChipsConfigs();
            BindLevels();
        }

        private void BindOptions()
        {
            Container.BindInterfacesTo<GameLevelsOptions>().AsSingle();
        }

        private void BindCameraRig()
        {
            Container.BindInstance(_cameraRigTransform).WithId(InjectionIds.Transform.CameraRig);
        }

        private void BindLayers()
        {
            Container.BindInstance(_chipLayer).WithId(InjectionIds.Int.ChipsLayer);
            Container.BindInstance(_ignoreRaycastsLayer).WithId(InjectionIds.Int.IgnoreRaycastsLayer);
        }

        private void BindChipsConfigs()
        {
            Container.BindAsync<ChipViewsConfig>().FromAssetReferenceT(_chipViewsConfigRef).AsCached();
            Container.BindAsync<ColoredChipsActivationConfig>().FromAssetReferenceT(_coloredChipsActivationConfigRef).AsCached();
        }

        private void BindLevels()
        {
            Container.BindInterfacesAndSelfTo<LevelsController>().AsSingle();
            Container.BindAsync<LevelsConfig>().FromAssetReferenceT(_levelsConfigRef).AsCached();
        }
    }
}