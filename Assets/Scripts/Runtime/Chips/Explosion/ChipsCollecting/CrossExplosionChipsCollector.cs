using System.Collections.Generic;
using Game.Level;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion.ChipsCollecting
{
    [UsedImplicitly]
    public class CrossExplosionChipsCollector : IExplosionChipsCollector
    {
        [Inject] private LevelModel _levelModel;
        [Inject] private IExplosionConfig _config;
        
        private CrossExplosionConfig Config => (CrossExplosionConfig)_config;
        
        public void Collect(Vector3 pivotPosition, List<ChipModel> results)
        {
            // TODO: Refactor to check collider size. Right now, it is inaccurate in case of a big collider (cause we are checking against pivot)
            var horizontalHitBounds = new Bounds(pivotPosition, new Vector2(Config.Size, Config.Thickness));
            var verticalHitBounds = new Bounds(pivotPosition, new Vector2(Config.Thickness, Config.Size));
            for (int i = 0; i < _levelModel.ChipModels.Count; i++)
            {
                var chipModel = _levelModel.ChipModels[i];
                var chipPosition = chipModel.View.transform.position;
                if (horizontalHitBounds.Contains(chipPosition))
                {
                    results.Add(chipModel);
                }
                else if (verticalHitBounds.Contains(chipPosition))
                {
                    results.Add(chipModel);
                }
            }
        }
    }
}