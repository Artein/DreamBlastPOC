using UnityEngine;
using Zenject;

namespace Game.Level.Stages
{
    public class LevelCameraInstaller : Installer<LevelCameraInstaller>
    {
        [Inject(Id = InjectionIds.Transform.CameraRig)] private Transform _cameraRigTransform;
        
        public override void InstallBindings()
        {
            _cameraRigTransform.transform.position = Vector3.zero;
        }
    }
}