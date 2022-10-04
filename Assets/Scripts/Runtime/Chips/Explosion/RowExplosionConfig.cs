using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion
{
    [CreateAssetMenu(fileName = nameof(RowExplosionConfig), menuName = "Game/Chips/Behaviour/Explosion/" + nameof(RowExplosionConfig), order = 0)]
    public class RowExplosionConfig : BaseExplosionConfig
    {
        [SerializeField, Min(0)] private float _impactWidth;
        [SerializeField, Min(0)] private float _impactHeight;

        public float ImpactWidth => _impactWidth;
        public float ImpactHeight => _impactHeight;

        public override IExplosionController InstantiateExplosionController(IInstantiator instantiator)
        {
            return instantiator.Instantiate<RowExplosionController>(new []{ this });
        }
    }
}