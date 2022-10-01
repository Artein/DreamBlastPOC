using System.Threading;
using Game.Level;
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
            Container.Bind<LevelModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
        }
    }
}