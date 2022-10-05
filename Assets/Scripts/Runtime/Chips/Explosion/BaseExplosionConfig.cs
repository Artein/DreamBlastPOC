using System;
using UnityEngine;

namespace Game.Chips.Explosion
{
    public abstract class BaseExplosionConfig : ScriptableObject, IExplosionConfig
    {
        public abstract Type ChipsCollectorType { get; }
    }
}