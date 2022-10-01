using System.Collections.Generic;
using Game.Chips;
using JetBrains.Annotations;

namespace Game.Level
{
    [UsedImplicitly]
    public class LevelModel
    {
        public readonly List<ChipModel> ChipModels = new();
    }
}