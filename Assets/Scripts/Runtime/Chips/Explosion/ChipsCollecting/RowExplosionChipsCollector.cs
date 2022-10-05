using System.Collections.Generic;
using Game.Level;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion.ChipsCollecting
{
    [UsedImplicitly]
    public class RowExplosionChipsCollector : IExplosionChipsCollector
    {
        [Inject] private LevelModel _levelModel;
        [Inject] private IExplosionConfig _config;

        private RowExplosionConfig Config => (RowExplosionConfig)_config;
        
        public void Collect(Vector3 pivotPosition, List<ChipModel> results)
        {
            var size = new Vector2(Config.ImpactWidth, Config.ImpactHeight);
            
            // TODO: Refactor to check collider size. Right now, it is inaccurate in case of a big collider (cause we are checking against pivot)
            var hitBounds = new Bounds(pivotPosition, size);
            for (int i = 0; i < _levelModel.ChipModels.Count; i++)
            {
                var chipModel = _levelModel.ChipModels[i];
                var chipPosition = chipModel.View.transform.position;
                if (hitBounds.Contains(chipPosition))
                {
                    results.Add(chipModel);
                }
            }
        }
    }
}