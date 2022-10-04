using JetBrains.Annotations;
using NativeSerializableDictionary;
using UnityEngine;

namespace Game.Chips
{
    [CreateAssetMenu(fileName = nameof(ChipViewsConfig), menuName = "Game/" + nameof(ChipViewsConfig), order = 0)]
    public class ChipViewsConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ChipId, ChipView> _chipViewPrefabs;

        public bool TryGetViewPrefab([NotNull] ChipId chipId, [CanBeNull] out ChipView prefab)
        {
            var found = _chipViewPrefabs.TryGetValue(chipId, out var serializablePair);
            prefab = found ? serializablePair.Value : null;
            return found;
        }
    }
}