using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Level
{
    [UsedImplicitly]
    public class LevelsController : IInitializable
    {
        private const string CurrentLevelIdxKey = "LevelsState.CurrentLevelIdx";
        [Inject] private AddressableInject<LevelsConfig> _levelsConfigAddressable;
        private LevelsConfig _levelsConfig;
        private IReadOnlyList<LevelConfig> Levels => _levelsConfig.Levels;
        
        public int CurrentLevelIdx => PlayerPrefs.GetInt(CurrentLevelIdxKey, 0);
        
        public LevelConfig CurrentLevel
        {
            get
            {
                if (Levels.Count > CurrentLevelIdx)
                {
                    return Levels[CurrentLevelIdx];
                }

                Debug.LogError($"[{nameof(LevelsController)}]: Attempt to get non existing level, {nameof(CurrentLevelIdx)}={CurrentLevelIdx}, LevelsCount={Levels.Count}");
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
            
            Debug.Log($"Level index changed to {CurrentLevelIdx} ({CurrentLevel.name})");
        }

        public async void Initialize()
        {
            _levelsConfig = await _levelsConfigAddressable;
            
            ValidateInitialState();
            Debug.Log($"Levels initialization, {nameof(CurrentLevelIdx)}={CurrentLevelIdx}, CurrentLevel.name='{CurrentLevel.name}'");
        }

        private void ValidateInitialState()
        {
            foreach (var level in Levels)
            {
                if (level == null)
                {
                    Debug.LogError($"NULL level found in {nameof(LevelsConfig)}");
                }
            }
        }
    }
}