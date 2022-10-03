using System.Threading;
using Game.Chips;
using Game.Input;
using Game.Platform;
using UnityEngine;
using Zenject;

namespace Game
{
    [DisallowMultipleComponent]
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        [SerializeField] private ChipViewsConfig _chipViewsConfig;
        [SerializeField] private ColoredChipsActivationConfig _coloredChipsActivationConfig;
        [SerializeField] private int _ignoreRaycastsLayer; // TODO: Validate value somehow (OdinInspector?)
        [SerializeField] private int _chipLayer; // TODO: Validate value somehow (OdinInspector?)
        
        private CancellationTokenSource _lifetimeCTS = new();

        private void OnDestroy()
        {
            _lifetimeCTS.Cancel();
            _lifetimeCTS.Dispose();
            _lifetimeCTS = null;
        }
        
        public override void InstallBindings()
        {
            Container.BindInstance(SRDebug.Instance).AsSingle();
            Container.BindInterfacesTo<TargetFPSController>().AsSingle();
            Container.BindInstance(_lifetimeCTS).AsSingle();
            Container.BindInstance(_chipViewsConfig).AsSingle();
            Container.BindInstance(_coloredChipsActivationConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<TouchInputNotifier>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameObjectInputNotifier>().AsSingle();
            Container.BindInstance(_chipLayer).WithId(InjectionIds.Value.ChipsLayer);
            Container.BindInstance(_ignoreRaycastsLayer).WithId(InjectionIds.Value.IgnoreRaycastsLayer);
        }
    }
}