using System;
using Game.Chips.Explosion.ChipsCollecting;
using TypeReferences;
using UnityEngine;

namespace Game.Chips.Explosion
{
    [CreateAssetMenu(fileName = nameof(RowExplosionConfig), menuName = "Game/Chips/Behaviour/Explosion/" + nameof(RowExplosionConfig), order = 0)]
    public class RowExplosionConfig : BaseExplosionConfig
    {
        [field: SerializeField, Min(0)]
        public float Width { get; set; } = 5;

        [field: SerializeField, Min(0)]
        public float Height { get; set; } = 1.2f;

        [SerializeField, Inherits(typeof(IExplosionChipsCollector), ShortName = true)] 
        private TypeReference _explosionChipsCollectorType;
        
        public override Type ChipsCollectorType => _explosionChipsCollectorType.Type;
    }
}