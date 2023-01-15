using Game.Level;
using Game.Utils.Addressable;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game
{
    public class CoreInstaller : MonoInstaller<CoreInstaller>
    {
        [SerializeField] private Transform _cameraRigTransform;
        [SerializeField, Layer] private int _ignoreRaycastsLayer;
        [SerializeField, Layer] private int _chipLayer;
        
        [Inject(Id = InjectionIds.AssetReferenceScene.Level)] private AssetReferenceScene _levelSceneRef;
        
        private void OnValidate()
        {
            Assert.IsNotNull(_cameraRigTransform, "_cameraRigTransform != null");
        }

        public override void Start()
        {
            var bootstrap = Container.Instantiate<Bootstrap>(new object[]{ _levelSceneRef });
            bootstrap.Execute();
        }
        
        public override void InstallBindings()
        {
            BindCameraRig();
            BindLayers();
            BindOptions();
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
    }
}