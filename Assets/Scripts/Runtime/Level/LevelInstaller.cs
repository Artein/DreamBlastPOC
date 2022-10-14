using System;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Game.Level
{
    [DisallowMultipleComponent]
    public class LevelInstaller : MonoInstaller<LevelInstaller>
    {
        [SerializeField] private Transform _levelContainer;
        [SerializeField] private Transform _cameraRig;

        [Inject] private LevelsController _levelsController;
        
        private CancellationTokenSource _lifetimeCTS = new();

        private void OnDestroy()
        {
            _lifetimeCTS.Cancel();
            _lifetimeCTS.Dispose();
            _lifetimeCTS = null;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_lifetimeCTS).AsSingle();
            Container.BindInstance(_levelsController.CurrentLevel).AsSingle();
            Container.BindInstance(_levelContainer).WithId(InjectionIds.Transform.LevelContainer);
            Container.BindInstance(_cameraRig).WithId(InjectionIds.Transform.CameraRig);
            Container.Bind(typeof(LevelOptions), typeof(IInitializable), typeof(IDisposable)).To<LevelOptions>().AsSingle();
            Container.Bind(typeof(ILevelConfig), typeof(IInitializable), typeof(IDisposable)).To<LevelConfigProxy>().AsSingle();
            Container.Bind<LevelModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
        }
    }
}