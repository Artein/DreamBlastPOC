using System;
using Game.Chips.Explosion.ChipsCollecting;
using TypeReferences;
using UnityEngine;

namespace Game.Chips.Explosion
{
    [CreateAssetMenu(fileName = nameof(RowExplosionConfig), menuName = "Game/Chips/Behaviour/Explosion/" + nameof(RowExplosionConfig), order = 0)]
    public class RowExplosionConfig : BaseExplosionConfig
    {
        [SerializeField, Min(0)] private float _impactWidth;
        [SerializeField, Min(0)] private float _impactHeight;
        
        [SerializeField, Inherits(typeof(IExplosionChipsCollector), ShortName = true)] 
        private TypeReference _explosionChipsCollectorType;

        public float ImpactWidth => _impactWidth;
        public float ImpactHeight => _impactHeight;
        public override Type ChipsCollectorType => _explosionChipsCollectorType.Type;
    }
}