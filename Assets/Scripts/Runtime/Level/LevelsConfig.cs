using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = nameof(LevelsConfig), menuName = "Game/Levels/" + nameof(LevelsConfig), order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private List<LevelConfig> _levels;

        public IReadOnlyList<LevelConfig> Levels => _levels.AsReadOnly();

        private void OnValidate()
        {
            for (int i = 0; i < Levels.Count; i += 1)
            {
                var level = Levels[i];
                if (level == null)
                {
                    Debug.LogError($"NULL level found (index: {i}). Remove entry or assign some level", this);
                }
            }
        }
    }
}