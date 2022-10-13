using System;
using JetBrains.Annotations;
using NativeSerializableDictionary;
using UnityEngine;

namespace Game.Chips
{
    [CreateAssetMenu(fileName = nameof(ChipViewsConfig), menuName = "Game/" + nameof(ChipViewsConfig), order = 0)]
    public class ChipViewsConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ChipId, Entry> _chipViewPrefabs;

        public bool TryGetViewPrefab([NotNull] ChipId chipId, [CanBeNull] out ChipView prefab)
        {
            var found = _chipViewPrefabs.TryGetValue(chipId, out var serializablePair);
            prefab = found ? serializablePair.Value.RegularView : null;
            return found;
        }

        public bool TryGetAnimatedViewPrefab([NotNull] ChipId chipId, [CanBeNull] out ChipView prefab)
        {
            var found = _chipViewPrefabs.TryGetValue(chipId, out var serializablePair);
            prefab = found ? serializablePair.Value.AnimatedView : null;
            return found;
        }

        [Serializable] private struct Entry
        {
            public ChipView RegularView;
            public ChipView AnimatedView;
        }
    }
}