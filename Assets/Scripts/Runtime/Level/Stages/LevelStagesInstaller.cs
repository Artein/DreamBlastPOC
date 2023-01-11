using Zenject;

namespace Game.Level.Stages
{
    public class LevelStagesInstaller : MonoInstaller<LevelStagesInstaller>
    {
        public override void InstallBindings()
        {
            LevelCameraInstaller.Install(Container);
            Container.Bind<LevelStagesController>().AsSingle();
        }
    }
}