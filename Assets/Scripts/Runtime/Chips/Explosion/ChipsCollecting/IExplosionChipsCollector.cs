using System.Collections.Generic;
using UnityEngine;

namespace Game.Chips.Explosion.ChipsCollecting
{
    public interface IExplosionChipsCollector
    {
        void Collect(Vector3 pivotPosition, List<ChipModel> results);
    }
}