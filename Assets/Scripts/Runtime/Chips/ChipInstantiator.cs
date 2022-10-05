using System;
using System.Collections.Generic;
using Game.Chips.Activation;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipInstantiator
    {
        [Inject] private ChipViewsConfig _chipViewsConfig;
        [Inject] private ChipActivatorsConfig _chipActivatorsConfig;
        [Inject] private DiContainer _instantiator;
        private ProfilerMarker _instantiateProfilerMarker = new($"{nameof(ChipInstantiator)}.{nameof(Instantiate)}");

        public ChipModel Instantiate(ChipId chipId, float3 position, Transform parent)
        {
            using var profileScopeHandle = _instantiateProfilerMarker.Auto();
            try
            {
                if (_chipViewsConfig.TryGetViewPrefab(chipId, out var viewPrefab))
                {
                    var chipView = _instantiator.InstantiatePrefabForComponent<ChipView>(viewPrefab, position, Quaternion.identity, parent);
                    var chipActivationExecutorType = _chipActivatorsConfig.FindExecutorType(chipId);
                    Assert.IsNotNull(chipActivationExecutorType); // TODO: Handle the case when chip can't be activated (executor == null)
                    
                    var chipIdTypeValuePair = new TypeValuePair(typeof(ChipId), chipId);
                    var activatorTypeValuePair = new TypeValuePair(typeof(IChipActivationExecutor), _instantiator.Instantiate(chipActivationExecutorType));
                    var chipModel = _instantiator.InstantiateExplicit<ChipModel>(new List<TypeValuePair>{ chipIdTypeValuePair, activatorTypeValuePair });
                    chipModel.View = chipView;

                    var sizeController = _instantiator.Instantiate<ChipSizeController>(new[] { chipModel });
                    ((IInitializable)sizeController).Initialize();
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