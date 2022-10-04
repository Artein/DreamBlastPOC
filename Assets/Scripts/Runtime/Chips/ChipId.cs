using System.Collections.Generic;
using Game.Chips.Tags;
using UnityEngine;

namespace Game.Chips
{
    [CreateAssetMenu(fileName = nameof(ChipId), menuName = "Game/Chips/" + nameof(ChipId), order = 0)]
    public class ChipId : ScriptableObject
    {
        [SerializeField] private List<ChipTag> _tags;

        public IReadOnlyList<ChipTag> Tags => _tags;
    }
}