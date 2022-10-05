using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = nameof(LevelConfig), menuName = "Game/Levels/" + nameof(LevelConfig), order = 0)]
    public class LevelConfig : ScriptableObject, ILevelConfig
    {
        [field: SerializeField, Min(0)]
        public int TotalChipsAmount { get; set; } = 20;

        [field: SerializeField, Min(0)]
        public float ChipsAmountCheckInterval { get; set; } = 1;

        [field: SerializeField] 
        public GameObject LevelTopologyPrefab { get; set; }
    }
}