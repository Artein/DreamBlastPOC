using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "Game/" + nameof(LevelConfig), order = 0)]
    public class LevelConfig : ScriptableObject, ILevelConfig
    {
        [SerializeField, Min(0)] private int _totalChipsAmount;
        [SerializeField, Min(0)] private float _chipsAmountCheckInterval;

        public int TotalChipsAmount => _totalChipsAmount;
        public float ChipsAmountCheckInterval => _chipsAmountCheckInterval;
    }
}