using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Game.Loading.UI;
using Game.Utils;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityUtils.State.Locking;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Loading
{
    [UsedImplicitly]
    public class LoaderPresenter : IInitializable, IDisposable
    {
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly SceneReference _loadingSceneRef;
        private readonly Loader _loader;
        private LoadingBarView _loadingBarView;
        private LoadingSceneView _loadingSceneView;

        public LoaderPresenter(Loader loader, [Inject(Id = InjectionIds.SceneReference.Loading)] SceneReference loadingSceneRef,
            ICancellationTokenProvider lifetimeCTProvider)
        {
            _lifetimeCTProvider = lifetimeCTProvider;
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
            _loader.Finishing -= OnLoadingFinishing;
            _loader.Starting -= OnLoadingStarting;
        }

        private void OnLoadingStarting(ILocker startLocker)
        {
            var startLockHandle = startLocker.Lock();
            _loader.Progress!.Changed += OnLoadingProgressChanged;
            
            // TODO: Animate canvas alpha
            LoadLoadingSceneAsync(_lifetimeCTProvider.Token)
                .ContinueWith(() => { startLockHandle.Dispose(); })
                .Forget();
        }

        private void OnLoadingFinishing(ILocker finishLocker)
        {
            var finishLockHandle = finishLocker.Lock();
            _loader.Progress!.Changed -= OnLoadingProgressChanged;
            
            // TODO: Animate canvas alpha
            UnloadLoadingSceneAsync(_lifetimeCTProvider.Token)
                .ContinueWith(() =>
                {
                    _loadingBarView = null;
                    _loadingSceneView = null;
                    finishLockHandle.Dispose();
                })
                .Forget();
        }

        private async UniTask LoadLoadingSceneAsync(CancellationToken cancellationToken)
        {
            var operation = SceneManager.LoadSceneAsync(_loadingSceneRef.Name, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            InitializeViews();
        }

        private void InitializeViews()
        {
            Assert.IsNull(_loadingBarView);
            _loadingBarView = Object.FindObjectOfType<LoadingBarView>(true);
            Assert.IsNull(_loadingSceneView);
            _loadingSceneView = Object.FindObjectOfType<LoadingSceneView>(true);
            _loadingSceneView.SetCanvasAlpha(0f);
        }

        private async UniTask UnloadLoadingSceneAsync(CancellationToken cancellationToken)
        {
            var operation = SceneManager.UnloadSceneAsync(_loadingSceneRef.Name);
            
            while (!operation.isDone)
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private void OnLoadingProgressChanged(float value, float prevValue)
        {
            if (_loadingBarView != null)
            {
                _loadingBarView.SetBarValue(value);
            }
        }
    }
}