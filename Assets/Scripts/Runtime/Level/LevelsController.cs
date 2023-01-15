using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Game.Level
{
    [ZenjectBound]
    public class LevelsController : IInitializable
    {
        private const string CurrentLevelIdxKey = "LevelsState.CurrentLevelIdx";
        [Inject] private AddressableInject<LevelsConfig> _levelsConfigAddressable;
        [Inject] private ICancellationTokenProvider _lifetimeCTProvider;
        private IReadOnlyList<LevelConfig> Levels => _levelsConfigAddressable.Result.Levels;
        
        public int CurrentLevelIdx => PlayerPrefs.GetInt(CurrentLevelIdxKey, 0);
        
        public LevelConfig CurrentLevel
        {
            get
            {
                if (Levels.Count > CurrentLevelIdx)
                {
                    return Levels[CurrentLevelIdx];
                }

                Debug.unityLogger.Log(LogType.Error, nameof(LevelsController), 
                    $"Attempt to get non existing level, {nameof(CurrentLevelIdx)}={CurrentLevelIdx}, LevelsCount={Levels.Count}");
                SetCurrentLevelIdx(0);
                return Levels[CurrentLevelIdx];
            }
        }

        public bool IsFinalLevel => CurrentLevelIdx + 1 >= Levels.Count;

        public bool TryIncrementCurrentLevel()
        {
            if (IsFinalLevel)
            {
                return false;
            }

            var nextLevelIdx = CurrentLevelIdx + 1;
            SetCurrentLevelIdx(nextLevelIdx);
            return true;
        }

        public void SetCurrentLevelIdx(int levelIdx)
        {
            if (levelIdx < 0 || levelIdx >= Levels.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(levelIdx), $"must be in range [0; {Levels.Count})");
            }
            
            PlayerPrefs.SetInt(CurrentLevelIdxKey, levelIdx);
            PlayerPrefs.Save();
            
            Debug.unityLogger.Log(nameof(LevelsController), $"Level index changed to {CurrentLevelIdx} ({CurrentLevel.name})");
        }

        public void Initialize()
        {
            InitializeAsync(_lifetimeCTProvider.Token).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            await _levelsConfigAddressable;
            cancellationToken.ThrowIfCancellationRequested();
            
            ValidateInitialState();
            Debug.unityLogger.Log(
                nameof(LevelsController), 
                $"Levels initialization, {nameof(CurrentLevelIdx)}={CurrentLevelIdx}, CurrentLevel.name='{CurrentLevel.name}'");
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void ValidateInitialState()
        {
            for (var i = 0; i < Levels.Count; i++)
            {
                var level = Levels[i];
                Debug.Assert(level != null, $"{nameof(LevelsController)}: NULL level found in {nameof(LevelsConfig)}");
            }
        }
    }
}