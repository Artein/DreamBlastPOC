using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Chips
{
    [DisallowMultipleComponent]
    public class ChipSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _chipsContainer;
        [SerializeField] private ChipId[] _chips;
        [SerializeField, Min(0)] private int _amount;
        [SerializeField, Min(0.01f)] private float _radius;
        [SerializeField, Min(0f)] private float _elementSpawnDelay;

        [Inject] private ChipViewsStorage _chipViewsStorage;
        [Inject] private IInstantiator _instantiator;
        [Inject] private CancellationTokenSource _lifetimeCTS;
        
        private void Start()
        {
            SpawnChipsAsync(_lifetimeCTS.Token).Forget();
        }

        private async UniTask SpawnChipsAsync(CancellationToken ct)
        {
            for (int i = 0; i < _amount; i++)
            {
                var chipIdx = Random.Range(0, _chips.Length);
                var chipId = _chips[chipIdx];
                
                if (_chipViewsStorage.TryGetViewPrefab(chipId, out var viewPrefab))
                {
                    var randomOffset = Random.insideUnitCircle * _radius;
                    var position = transform.position;
                    position.x += randomOffset.x;
                    position.y += randomOffset.y;
                    _instantiator.InstantiatePrefab(viewPrefab, position, Quaternion.identity, _chipsContainer);
                }
                else
                {
                    Debug.LogError($"{nameof(ChipViewsStorage)} missing setup for '{chipId.name}'", _chipViewsStorage);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_elementSpawnDelay), cancellationToken: ct);
                ct.ThrowIfCancellationRequested();
            }
        }
    }
}