using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = nameof(LevelsConfig), menuName = "Game/Levels/" + nameof(LevelsConfig), order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private List<LevelConfig> _levels;

        public IReadOnlyList<LevelConfig> Levels => _levels;

        private void OnValidate()
        {
            foreach (var level in Levels)
            {
                if (level == null)
                {
                    Debug.LogError("NULL level found. Remove entry or assign some level", this);
                }
            }
        }
    }
}