using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TypeReferences;
using UnityEngine;

namespace Game.Chips.Activation
{
    [CreateAssetMenu(fileName = nameof(ChipActivatorsConfig), menuName = "Game/Chips/" + nameof(ChipActivatorsConfig), order = 0)]
    public class ChipActivatorsConfig : ScriptableObject
    {
        [SerializeField] private List<ChipActivationExecutorEntry> _activationExecutors;
        
        [CanBeNull] public Type FindExecutorType([NotNull] ChipId chipId)
        {
            for (int i = 0; i < _activationExecutors.Count; i++)
            {
                var entry = _activationExecutors[i];
                if (entry.ChipId == chipId)
                {
                    return entry.ExecutorTypeRef.Type;
                }
            }

            return null;
        }
        
        [Serializable] public struct ChipActivationExecutorEntry : IEquatable<ChipActivationExecutorEntry>
        {
            public ChipId ChipId;
            [Inherits(typeof(IChipActivationExecutor), ShortName = true)] public TypeReference ExecutorTypeRef;

            public bool Equals(ChipActivationExecutorEntry other)
            {
                return ExecutorTypeRef.Equals(other.ExecutorTypeRef);
            }

            public override bool Equals(object obj)
            {
                return obj is ChipActivationExecutorEntry other && Equals(other);
            }

            public override int GetHashCode()
            {
                return ExecutorTypeRef.GetHashCode();
            }
        }
    }
}