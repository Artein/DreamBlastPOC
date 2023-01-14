using System.Collections.Generic;
using Game.Level;
using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion.ChipsCollecting
{
    [ZenjectBound]
    public class RowExplosionChipsCollector : IExplosionChipsCollector
    {
        [Inject] private LevelModel _levelModel;
        [Inject] private IExplosionConfig _config;

        private RowExplosionConfig Config => (RowExplosionConfig)_config;
        
        public void Collect(Vector3 pivotPosition, List<ChipModel> results)
        {
            // TODO: Refactor to check collider size. Right now, it is inaccurate in case of a big collider (cause we are checking against pivot)
            var hitBounds = new Bounds(pivotPosition, new Vector2(Config.Width, Config.Height));
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