using System;
using Eflatun.SceneReference;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityUtils.State.Locking;
using Zenject;

namespace Game.Loading
{
    [UsedImplicitly]
    public class LoaderPresenter : IInitializable, IDisposable
    {
        private readonly Loader _loader;
        private readonly SceneReference _loadingSceneRef;

        public LoaderPresenter(Loader loader, [Inject(Id = InjectionIds.SceneReference.Loading)] SceneReference loadingSceneRef)
        {
            _loadingSceneRef = loadingSceneRef;
            _loader = loader;
        }
        
        void IInitializable.Initialize()
        {
            _loader.Starting += OnLoadingStarting;
            _loader.Finishing += OnLoadingFinishing;
        }

        void IDisposable.Dispose()
        {
            _loader.Starting -= OnLoadingStarting;
        }

        private async void OnLoadingStarting(ILocker startLocker)
        {
            using var startLockHandle = startLocker.Lock();
            
            var sceneLoadingOperationHandle = SceneManager.LoadSceneAsync(_loadingSceneRef.Name, LoadSceneMode.Additive);
            sceneLoadingOperationHandle.allowSceneActivation = false;
        }

        private void OnLoadingFinishing(ILocker finishLocker)
        {
            throw new NotImplementedException();
        }
    }
}