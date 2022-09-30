using UnityEngine;

namespace Game.Chips
{
    [CreateAssetMenu(fileName = nameof(ChipId), menuName = "Game/" + nameof(ChipId), order = 0)]
    public class ChipId : ScriptableObject
    {
        [SerializeField] private ColorId _color;

        public ColorId Color => _color;
    }
}