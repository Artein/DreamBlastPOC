using JetBrains.Annotations;
using Zenject;

namespace Game.Chips.Explosion
{
    public interface IExplosionConfig
    {
        IExplosionController InstantiateExplosionController([NotNull] IInstantiator instantiator);
    }
}