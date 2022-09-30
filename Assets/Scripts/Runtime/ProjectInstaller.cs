using System.Threading;
using Game.Chips;
using UnityEngine;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [SerializeField] private ChipViewsStorage _chipViewsStorage;
        
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
            Container.BindInstance(_chipViewsStorage).AsSingle();
        }
    }
}