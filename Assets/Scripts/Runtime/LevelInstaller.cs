using System;
using System.Threading;
using Game.Level;
using Game.Options;
using UnityEngine;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class LevelInstaller : MonoInstaller<LevelInstaller>
    {
        [SerializeField] private LevelConfig _levelConfig;
        
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
            Container.BindInstance(_levelConfig).AsSingle();
            Container.Bind(typeof(LevelOptions), typeof(IInitializable), typeof(IDisposable)).To<LevelOptions>().AsSingle();
            Container.Bind(typeof(ILevelConfig), typeof(IInitializable), typeof(IDisposable)).To<LevelConfigProxy>().AsSingle();
            Container.Bind<LevelModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
        }
    }
}