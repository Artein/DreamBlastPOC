using System.Threading;
using UnityEngine;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class SceneInstaller : MonoInstaller<SceneInstaller>
    {
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
        }
    }
}