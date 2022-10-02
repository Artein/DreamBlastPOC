using System;
using System.Collections.Generic;
using Game.Chips.Activation;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipInstantiator
    {
        [Inject] private ChipViewsConfig _chipViewsConfig;
        [Inject] private DiContainer _instantiator;

        public ChipModel Instantiate(ChipId chipId, float3 position, Transform parent)
        {
            try
            {
                if (_chipViewsConfig.TryGetViewPrefab(chipId, out var viewPrefab))
                {
                    var chipView = _instantiator.InstantiatePrefab(viewPrefab, position, Quaternion.identity, parent);
                    var chipIdTypeValuePair = new TypeValuePair(typeof(ChipId), chipId);
                    // TODO: Update when new chip types will be added
                    var activatorTypeValuePair = new TypeValuePair(typeof(IChipActivationExecutor), _instantiator.Instantiate<ColoredChipsBlobActivationExecutor>());
                    var chipModel = _instantiator.InstantiateExplicit<ChipModel>(new List<TypeValuePair>{ chipIdTypeValuePair, activatorTypeValuePair });
                    chipModel.View = chipView;
                    return chipModel;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }

            Debug.LogError($"{nameof(ChipViewsConfig)} missing setup for '{chipId.name}'", _chipViewsConfig);
            throw new InvalidOperationException();
        }
    }
}