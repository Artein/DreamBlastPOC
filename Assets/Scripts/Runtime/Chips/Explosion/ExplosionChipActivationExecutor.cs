using System.Collections.Generic;
using Game.Chips.Activation;
using Game.Chips.Explosion.ChipsCollecting;
using Game.Level;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using Zenject;

namespace Game.Chips.Explosion
{
    [UsedImplicitly]
    public class ExplosionChipActivationExecutor : IChipActivationExecutor
    {
        [Inject] private ExplosionChipsConfig _explosionChipsConfig;
        [Inject] private LevelModel _levelModel;
        [Inject] private IInstantiator _instantiator;

        private readonly List<ChipModel> _hitChipModels = new();
        
        // TODO Optimization: Use jobs
        public bool TryActivate(ChipModel pivotChipModel)
        {
            var foundExplosionConfig = _explosionChipsConfig.TryGetExplosionConfig(pivotChipModel.ChipId, out var explosionConfig);
            if (foundExplosionConfig)
            {
                Assert.IsTrue(_hitChipModels.Count == 0);
                var chipsCollector = (IExplosionChipsCollector)_instantiator.Instantiate(explosionConfig.ChipsCollectorType, new[] { explosionConfig });
                chipsCollector.Collect(pivotChipModel.View.transform.position, _hitChipModels);

                for (int i = 0; i < _hitChipModels.Count; i++)
                {
                    var chipModel = _hitChipModels[i];
                    _levelModel.ChipModels.Remove(chipModel);
                    
                    // TODO: Usually we would like to Impact those chips. After impact it might be activated too, but for now just Destroy
                    chipModel.Destroy();
                }
            }
            
            _hitChipModels.Clear();

            return foundExplosionConfig;
        }
    }
}