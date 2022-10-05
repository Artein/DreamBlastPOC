using System.Linq;
using Game.Level;
using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Chips.Explosion.Debug
{
    [RequireComponent(typeof(ChipView))]
    public class Debug_DrawRowExplosionHitAreaGizmo : MonoBehaviour
    {
        [Inject] private ExplosionChipsConfig _explosionChipsConfig;
        [Inject] private LevelModel _levelModel;

        private ChipId _chipId;

        private void Start()
        {
            var chipView = GetComponent<ChipView>();
            var chipModel = _levelModel.ChipModels.First(chipModel => chipModel.View == chipView);
            _chipId = chipModel.ChipId;
        }

        private void OnDrawGizmos()
        {
            if (_explosionChipsConfig.TryGetExplosionConfig(_chipId, out var explosionConfig))
            {
                var rowExplosionConfig = (RowExplosionConfig)explosionConfig;
                var size = new Vector3(rowExplosionConfig.ImpactWidth, rowExplosionConfig.ImpactHeight);
                using var setColorHandle = GizmoUtils.SetColorWithHandle(Color.yellow);
                Gizmos.DrawWireCube(transform.position, size);
            }
        }
    }
}