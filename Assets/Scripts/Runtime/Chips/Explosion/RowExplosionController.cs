using JetBrains.Annotations;
using Zenject;

namespace Game.Chips.Explosion
{
    [UsedImplicitly]
    public class RowExplosionController : IExplosionController
    {
        [Inject] private RowExplosionConfig _explosionConfig;
    }
}