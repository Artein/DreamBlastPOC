using System.Threading;
using Eflatun.SceneReference;
using Game.Input;
using Game.Platform;
using Game.Utils;
using Game.Utils.Addressable;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [SerializeField] private SceneReference _coreSceneRef;
        [SerializeField] private AssetReferenceScene _levelSceneRef;
        
        private CancellationTokenSource _lifetimeCTS = new();

        private void OnValidate()
        {
            _levelSceneRef.RuntimeKeyIsValid();
            Assert.IsTrue(_coreSceneRef.HasValue);
        }

        private void OnDestroy()
        {
            _lifetimeCTS.Cancel();
            _lifetimeCTS.Dispose();
            _lifetimeCTS = null;
        }

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.Bind<ICancellationTokenProvider>().FromInstance(new CancellationTokenProvider(_lifetimeCTS)).AsSingle();
            Container.BindInterfacesTo<TargetFPSController>().AsSingle();
            
            BindOptions();
            BindInput();

            Container.BindInstance(_coreSceneRef).WithId(InjectionIds.SceneReference.Core).AsCached().NonLazy();
            Container.BindInstance(_levelSceneRef).WithId(InjectionIds.AssetReferenceScene.Level).AsCached().NonLazy();
        }

        private void BindOptions()
        {
            Container.BindInstance(SRDebug.Instance).AsSingle();
            Container.BindInterfacesTo<ProjectOptions>().AsSingle();
        }

        private void BindInput()
        {
            Container.BindInterfacesAndSelfTo<TouchInputNotifier>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameObjectInputNotifier>().AsSingle();
        }
    }
}