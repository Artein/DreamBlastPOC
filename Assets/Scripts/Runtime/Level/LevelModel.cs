using System.Collections.Generic;
using Game.Chips;
using Game.Utils;

namespace Game.Level
{
    [ZenjectBound]
    public class LevelModel
    {
        public readonly List<ChipModel> ChipModels = new();
    }
}