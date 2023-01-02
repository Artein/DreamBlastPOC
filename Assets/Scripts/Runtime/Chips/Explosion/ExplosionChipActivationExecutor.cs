using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Chips.Activation;
using Game.Chips.Explosion.ChipsCollecting;
using Game.Level;
using Game.Utils;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion
{
    [UsedImplicitly]
    public class ExplosionChipActivationExecutor : IChipActivationExecutor
    {
        [Inject] private ExplosionChipsConfig _explosionChipsConfig;
        [Inject] private LevelModel _levelModel;
        [Inject] private IInstantiator _instantiator;
        [Inject] private ICancellationTokenProvider _lifetimeCTProvider;
        
        // TODO Optimization: Use jobs
        public bool TryActivate(ChipModel pivotChipModel)
        {
            var foundExplosionConfig = _explosionChipsConfig.TryGetExplosionConfig(pivotChipModel.ChipId, out var explosionConfig);
            if (foundExplosionConfig)
            {
                PlayExplosionSequenceAsync(pivotChipModel, explosionConfig, _lifetimeCTProvider.Token).Forget();
            }

            return foundExplosionConfig;
        }

        private async UniTask PlayExplosionSequenceAsync(ChipModel explosiveChip, IExplosionConfig explosionConfig, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            bool explosiveChipDestroyed = false;
            var explosionPivotPosition = Vector3.zero;
            explosiveChip.Destroyed += () =>
            {
                explosionPivotPosition = explosiveChip.View.transform.position;
                explosiveChipDestroyed = true;
            }; // TODO: Multiple subscription issue in case using pools
            _levelModel.ChipModels.Remove(explosiveChip);
            explosiveChip.Destroy();

            await UniTask.WaitUntil(() => explosiveChipDestroyed, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            
            List<ChipModel> hitChips = new();
            var chipsCollector = (IExplosionChipsCollector)_instantiator.Instantiate(explosionConfig.ChipsCollectorType, new[] { explosionConfig });
            chipsCollector.Collect(explosionPivotPosition, hitChips);

            foreach (var hitChip in hitChips)
            {
                _levelModel.ChipModels.Remove(hitChip);
                    
                // TODO: Usually we would like to Impact those chips. After impact it might be activated too, but for now just Destroy
                hitChip.Destroy();
            }
        }
    }
}