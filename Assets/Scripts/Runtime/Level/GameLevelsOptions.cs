using System;
using Cysharp.Threading.Tasks;
using Game.Helpers;
using JetBrains.Annotations;
using ModestTree;
using SRDebugger;
using SRDebugger.Services;
using SRF.Helpers;
using Zenject;

namespace Game.Level
{
    [UsedImplicitly]
    public class GameLevelsOptions : IInitializable, IDisposable
    {
        [Inject] private ActiveSceneReloadCommand _activeSceneReloadCommand;

        private DynamicOptionContainer _container;
        [Inject] private IDebugService _debugService;
        [Inject] private LevelsController _levelsController;
        [Inject] private LevelsConfig _levelsConfig;

        void IInitializable.Initialize()
        {
            _debugService.AddOptionContainer(this);

            Assert.IsNull(_container);
            _container = new DynamicOptionContainer();
            _debugService.AddOptionContainer(_container);

            for (var i = 0; i < _levelsConfig.Levels.Count; i += 1)
            {
                var level = _levelsConfig.Levels[i];
                var i1 = i;
                var optionDefinition = new OptionDefinition(level.name, "Levels", 0, new MethodReference(args =>
                                                                                                         {
                                                                                                             LevelSelected(i1);
                                                                                                             return null;
                                                                                                         }));
                _container.AddOption(optionDefinition);
            }
        }

        void IDisposable.Dispose()
        {
            Assert.IsNotNull(_container);
            _debugService.RemoveOptionContainer(_container);
            _container = null;

            _debugService.RemoveOptionContainer(this);
        }

        private void LevelSelected(int levelIdx)
        {
            if (levelIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(levelIdx), levelIdx, $"Didn't find selected level in {nameof(LevelsConfig)}");
            }

            _levelsController.SetCurrentLevelIdx(levelIdx);
            _debugService.HideDebugPanel();
            _activeSceneReloadCommand.ExecuteAsync().Forget();
        }
    }
}