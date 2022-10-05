using UnityEngine;

namespace Game.Level
{
    public interface ILevelConfig
    {
        int TotalChipsAmount { get; }
        float ChipsAmountCheckInterval { get; }
        GameObject LevelTopologyPrefab { get; }
    }
}