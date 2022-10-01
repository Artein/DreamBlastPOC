using System;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipInstantiator
    {
        [Inject] private ChipViewsStorage _chipViewsStorage;
        [Inject] private IInstantiator _instantiator;

        public ChipModel Instantiate(ChipId chipId, float3 position, Transform parent)
        {
            if (_chipViewsStorage.TryGetViewPrefab(chipId, out var viewPrefab))
            {
                var chipView = _instantiator.InstantiatePrefab(viewPrefab, position, Quaternion.identity, parent);
                var chipModel = _instantiator.Instantiate<ChipModel>();
                chipModel.View = chipView;
                return chipModel;
            }

            Debug.LogError($"{nameof(ChipViewsStorage)} missing setup for '{chipId.name}'", _chipViewsStorage);
            throw new InvalidOperationException();
        }
    }
}