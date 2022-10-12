using Zenject;

namespace Game.Level.Stages
{
    public class LevelStagesInstaller : MonoInstaller<LevelStagesInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<LevelStagesController>().AsSingle();
        }
    }
}