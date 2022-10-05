using System;
using Game.Chips.Explosion.ChipsCollecting;
using TypeReferences;
using UnityEngine;

namespace Game.Chips.Explosion
{
    [CreateAssetMenu(fileName = nameof(CrossExplosionConfig), menuName = "Game/Chips/Behaviour/Explosion/" + nameof(CrossExplosionConfig), order = 0)]
    public class CrossExplosionConfig : BaseExplosionConfig
    {
        [field: SerializeField, Min(0)]
        public float Size { get; set; }

        [field: SerializeField, Min(0)]
        public float Thickness { get; set; }

        [SerializeField, Inherits(typeof(IExplosionChipsCollector), ShortName = true)] 
        private TypeReference _chipsCollectorType;

        public override Type ChipsCollectorType => _chipsCollectorType.Type;
    }
}