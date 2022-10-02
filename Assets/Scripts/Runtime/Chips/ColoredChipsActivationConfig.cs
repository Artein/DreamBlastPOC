using UnityEngine;

namespace Game.Chips
{
    [CreateAssetMenu(fileName = nameof(ColoredChipsActivationConfig), menuName = "Game/" + nameof(ColoredChipsActivationConfig), order = 0)]
    public class ColoredChipsActivationConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int _similarColorMinMatchSize;
        [SerializeField, Min(0)] private float _chipMatchRadius;

        public int SimilarColorMinMatchSize => _similarColorMinMatchSize;
        public float ChipMatchRadius => _chipMatchRadius;
    }
}