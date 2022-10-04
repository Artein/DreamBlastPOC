using JetBrains.Annotations;
using NativeSerializableDictionary;
using UnityEngine;

namespace Game.Chips
{
    [CreateAssetMenu(fileName = nameof(ColoredChipsActivationConfig), menuName = "Game/Chips/" + nameof(ColoredChipsActivationConfig), order = 0)]
    public class ColoredChipsActivationConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int _similarColorMinMatchSize;
        [SerializeField, Min(0)] private float _chipMatchRadius;
        [SerializeField, Tooltip("When match size is >= certain value => create chip at pivot position")]
        private SerializableDictionary<int, ChipId> _afterMatchChipCreationDic;

        public int SimilarColorMinMatchSize => _similarColorMinMatchSize;
        public float ChipMatchRadius => _chipMatchRadius;

        public bool TryGetChipToCreateAfterMatch(int matchSize, [CanBeNull] out ChipId chipId)
        {
            chipId = null;
            
            // expecting dictionary already sorted by key
            foreach (var serializablePair in _afterMatchChipCreationDic.Values)
            {
                var chipMatchSize = serializablePair.Key;
                if (matchSize >= chipMatchSize)
                {
                    chipId = serializablePair.Value;
                }
                else
                {
                    break;
                }
            }

            return chipId != null;
        }
    }
}