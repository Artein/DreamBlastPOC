using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading;
using Game.Loading.Tasks;
using Game.Utils;
using Game.Utils.Addressable;
using SRDebugger;
using SRDebugger.Services;
using SRF.Helpers;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Level
{
    [ZenjectBound]
    public class GameLevelsOptions : IInitializable, IDisposable
    {
        [Inject] private Loader _loader;
        [Inject] private IDebugService _debugService;
        [Inject] private LevelsController _levelsController;
        [Inject] private ICancellationTokenProvider _lifetimeCTProvider;
        [Inject] private AddressableScenesStorage _addressableScenesStorage;
        [Inject] private AddressableInject<LevelsConfig> _levelsConfigAddressable;

        [Inject(Id = InjectionIds.AssetReferenceScene.Level)]
        private AssetReferenceScene _levelSceneRef;
        
        private DynamicOptionContainer _container;

        private LevelsConfig LevelsConfig => _levelsConfigAddressable.Result;

        void IInitializable.Initialize()
        {
            InitializeAsync(_lifetimeCTProvider.Token).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            await _levelsConfigAddressable;
            cancellationToken.ThrowIfCancellationRequested();
            
            _debugService.AddOptionContainer(this);

            Assert.IsNull(_container);
            _container = new DynamicOptionContainer();
            _debugService.AddOptionContainer(_container);

            for (var i = 0; i < LevelsConfig.Levels.Count; i += 1)
            {
                var level = LevelsConfig.Levels[i];
                var i1 = i;
                var methodRef = new MethodReference(_ =>
                {
                    LevelSelected(i1);
                    return null;
                });
                var optionDefinition = new OptionDefinition(level.name, "Levels", 0, methodRef);
                _container.AddOption(optionDefinition);
            }
        }

        void IDisposable.Dispose()
        {
            if (_container != null)
            {
                _debugService.RemoveOptionContainer(_container);
                _container = null;

                _debugService.RemoveOptionContainer(this);
            }
        }

        private void LevelSelected(int levelIdx)
        {
            if (levelIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(levelIdx), levelIdx, $"Didn't find selected level in {nameof(LevelsConfig)}");
            }

            _levelsController.SetCurrentLevelIdx(levelIdx);
            _debugService.HideDebugPanel();

            _loader.Reset();
            _loader.Enqueue(CreateReleasePreviouslyLoadedLevelSceneTask())
                .Enqueue(CreateLoadLevelSceneTask())
                .StartAsync(_lifetimeCTProvider.Token).Forget();
        }

        private WeightedLoadingTask CreateLoadLevelSceneTask()
        {
            var task = new LoadAddressableSceneTask(_addressableScenesStorage, _levelSceneRef, LoadSceneMode.Additive);
            return new WeightedLoadingTask(task);
        }

        private WeightedLoadingTask CreateReleasePreviouslyLoadedLevelSceneTask()
        {
            var handle = _addressableScenesStorage.TakeLoadOperation(_levelSceneRef);
            Assert.IsTrue(handle.HasValue);
            var task = new ReleaseAddressableHandleTask<SceneInstance>(handle.Value);
            return new WeightedLoadingTask(task);
        }
    }
}