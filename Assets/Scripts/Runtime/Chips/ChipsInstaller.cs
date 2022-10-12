using Game.Chips.Activation;
using Game.Chips.Explosion;
using UnityEngine;
using Zenject;

namespace Game.Chips
{
    public class ChipsInstaller : MonoInstaller<ChipsInstaller>
    {
        [SerializeField] private Transform _chipsContainer;
        [SerializeField] private ChipActivatorsConfig _chipActivatorsConfig;
        [SerializeField] private ExplosionChipsConfig _explosionChipsConfig;
        
        public override void InstallBindings()
        {
            Container.DeclareSignal<ChipsMatchPerformedSignal>();
            Container.Bind<ChipInstantiator>().AsSingle();
            Container.BindInterfacesTo<ChipByUserInputActivator>().AsSingle();
            Container.BindInstance(_chipsContainer).WithId(InjectionIds.Transform.ChipsContainer);
            Container.BindInstance(_chipActivatorsConfig).AsSingle();
            Container.BindInstance(_explosionChipsConfig).AsSingle();
        }
    }
}