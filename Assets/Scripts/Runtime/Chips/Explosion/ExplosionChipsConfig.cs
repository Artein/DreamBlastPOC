using Game.Chips.Tags;
using JetBrains.Annotations;
using NativeSerializableDictionary;
using UnityEngine;

namespace Game.Chips.Explosion
{
    [CreateAssetMenu(fileName = nameof(ExplosionChipsConfig), menuName = "Game/Chips/Behaviour/Explosion/" + nameof(ExplosionChipsConfig), order = 0)]
    public class ExplosionChipsConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<ExplosionChipTag, BaseExplosionConfig> _configsDic;

        public bool TryGetExplosionConfig([NotNull] ExplosionChipTag chipTag, out IExplosionConfig explosionConfig)
        {
            if (_configsDic.TryGetValue(chipTag, out var configPair))
            {
                explosionConfig = configPair.Value;
                return true;
            }

            explosionConfig = null;
            return false;
        }
    }
}