using System;
using System.Collections.Generic;
using Game.Chips.Activation;
using Game.Utils;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game.Chips
{
    [ZenjectBound]
    public class ChipInstantiator
    {
        [Inject] private AddressableInject<ChipViewsConfig> _chipViewsConfigAddressable;
        [Inject] private ChipActivatorsConfig _chipActivatorsConfig;
        [Inject] private DiContainer _instantiator;
        [Inject] private ChipsOptions _chipsOptions;
        private ProfilerMarker _instantiateProfilerMarker = new($"{nameof(ChipInstantiator)}.{nameof(Instantiate)}");
        private ChipViewsConfig ChipViewsConfig => _chipViewsConfigAddressable.Result;

        public ChipModel Instantiate(ChipId chipId, float3 position, Transform parent)
        {
            using var profileScopeHandle = _instantiateProfilerMarker.Auto();
            Assert.IsTrue(_chipViewsConfigAddressable.HasResult);
            
            try
            {
                ChipView viewPrefab = null;
                bool foundViewPrefab = false;
                if (_chipsOptions.AnimatedChips)
                {
                    foundViewPrefab = ChipViewsConfig.TryGetAnimatedViewPrefab(chipId, out viewPrefab);
                }
                else
                {
                    foundViewPrefab = ChipViewsConfig.TryGetViewPrefab(chipId, out viewPrefab);
                }
                
                if (foundViewPrefab)
                {
                    var chipActivationExecutorType = _chipActivatorsConfig.FindExecutorType(chipId);
                    Assert.IsNotNull(chipActivationExecutorType); // TODO: Handle the case when chip can't be activated (executor == null)
                    
                    var chipIdTypeValuePair = new TypeValuePair(typeof(ChipId), chipId);
                    var activatorTypeValuePair = new TypeValuePair(typeof(IChipActivationExecutor), _instantiator.Instantiate(chipActivationExecutorType));
                    var chipModel = _instantiator.InstantiateExplicit<ChipModel>(new List<TypeValuePair>{ chipIdTypeValuePair, activatorTypeValuePair });
                    chipModel.View = _instantiator.InstantiatePrefabForComponent<ChipView>(viewPrefab, position, Quaternion.identity, parent, new [] { chipModel });

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

            Debug.LogError($"{nameof(ChipViewsConfig)} missing setup for '{chipId.name}'", ChipViewsConfig);
            throw new InvalidOperationException();
        }
    }
}