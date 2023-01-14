using Eflatun.SceneReference;
using Game.Loading.Tasks;
using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Loading
{
    [ZenjectBound]
    public class ProjectLoadingInstaller : MonoInstaller<ProjectLoadingInstaller>
    {
        [SerializeField] private SceneReference _loadingScene;
        
        public override void InstallBindings()
        {
            Container.BindInstance(_loadingScene).WithId(InjectionIds.SceneReference.Loading).AsTransient();
            Container.Bind<Loader>().AsSingle();
            Container.BindInterfacesTo<LoaderPresenter>().AsSingle();
            Container.Bind<LoadAddressableSceneTask>().AsTransient();
            Container.Bind<AddressableScenesStorage>().AsSingle();
        }
    }
}