using JetBrains.Annotations;
using Zenject;

namespace Game.Loading
{
    [UsedImplicitly]
    public class ProjectLoadingInstaller : Installer<ProjectLoadingInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneLoadingTask>().AsTransient();
        }
    }
}