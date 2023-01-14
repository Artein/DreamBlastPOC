using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Game.Loading.UI;
using Game.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityUtils.State.Locking;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Loading
{
    [ZenjectBound]
    public class LoaderPresenter : IInitializable, ITickable, IDisposable
    {
        private readonly ICancellationTokenProvider _lifetimeCTProvider;
        private readonly SceneReference _loadingSceneRef;
        private readonly Loader _loader;
        private LoadingBarView _loadingBarView;
        private LoadingSceneView _loadingSceneView;
        private float _smoothedProgress01;
        private float _smoothedProgress01Velocity;

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

        void ITickable.Tick()
        {
            if (_loader.IsLoading && _loadingBarView != null)
            {
                _smoothedProgress01 = Mathf.SmoothDamp(
                    _smoothedProgress01, _loader.Progress!.Progress01, ref _smoothedProgress01Velocity, _loadingBarView.SmoothAnimationTime);
                _loadingBarView.SetBarValue(_smoothedProgress01);
            }
        }

        private void OnLoadingStarting(ILocker startLocker)
        {
            var startLockHandle = startLocker.Lock();
            _smoothedProgress01 = 0f;
            
            LoadLoadingSceneAsync(_lifetimeCTProvider.Token)
                .ContinueWith(() =>
                {
                    _loadingSceneView.SetCanvasAlpha(1f);
                    startLockHandle.Dispose();
                })
                .Forget();
        }

        private void OnLoadingFinishing(ILocker finishLocker)
        {
            var finishLockHandle = finishLocker.Lock();
            
            _loadingSceneView.StartPlayingDisappearAnimationAsync(_lifetimeCTProvider.Token)
                .ContinueWith(() => UnloadLoadingSceneAsync(_lifetimeCTProvider.Token))
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
            cancellationToken.ThrowIfCancellationRequested();
            var operation = SceneManager.LoadSceneAsync(_loadingSceneRef.Name, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
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
            cancellationToken.ThrowIfCancellationRequested();
            var operation = SceneManager.UnloadSceneAsync(_loadingSceneRef.Name);
            
            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            }
        }
    }
}