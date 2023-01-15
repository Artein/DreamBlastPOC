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
        
        private void OnValidate()
        {
            Assert.IsNotNull(_cameraRigTransform, "_cameraRigTransform != null");
        }
        
        public override void InstallBindings()
        {
            BindCameraRig();
            BindLayers();
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