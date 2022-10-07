using Zenject;

namespace Game.Physics
{
    public class PhysicsSceneInstaller : MonoInstaller<PhysicsSceneInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PhysicsOptions>().AsSingle();
            Container.Bind<PhysicsSimulationExecutor>().FromComponentInHierarchy().AsSingle();
        }
    }
}