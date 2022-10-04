using UnityEngine;

namespace Game.Chips.Tags
{
    [CreateAssetMenu(fileName = nameof(ColorChipTag), menuName = "Game/Chips/Tags/" + nameof(ColorChipTag), order = 0)]
    public class ColorChipTag : ChipTag
    {
        [SerializeField] private ColorId _color;

        public ColorId Color => _color;
    }
}