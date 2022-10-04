using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion
{
    public abstract class BaseExplosionConfig : ScriptableObject, IExplosionConfig
    {
        public abstract IExplosionController InstantiateExplosionController(IInstantiator instantiator);
    }
}