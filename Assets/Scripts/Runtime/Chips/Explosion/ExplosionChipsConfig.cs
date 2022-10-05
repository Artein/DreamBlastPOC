using JetBrains.Annotations;
using NativeSerializableDictionary;
using UnityEngine;

namespace Game.Chips.Explosion
{
    [CreateAssetMenu(fileName = nameof(ExplosionChipsConfig), menuName = "Game/Chips/Behaviour/Explosion/" + nameof(ExplosionChipsConfig), order = 0)]
    public class ExplosionChipsConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ChipId, BaseExplosionConfig> _configsDic;

        public bool TryGetExplosionConfig([NotNull] ChipId chipId, out IExplosionConfig explosionConfig)
        {
            if (_configsDic.TryGetValue(chipId, out var configPair))
            {
                explosionConfig = configPair.Value;
                return true;
            }

            explosionConfig = null;
            return false;
        }
    }
}