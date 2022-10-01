using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Level;
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
        [SerializeField, Min(0.01f)] private float _radius;
        [SerializeField, Min(0f)] private float _elementSpawnDelay;

        [Inject] private LevelModel _levelModel;
        [Inject] private LevelController _levelController;
        [Inject] private ChipInstantiator _chipInstantiator;

        private void OnEnable()
        {
            _levelController.ChipsSpawningRequested += OnLevelRequestedChipsSpawning;
        }

        private void OnDisable()
        {
            _levelController.ChipsSpawningRequested -= OnLevelRequestedChipsSpawning;
        }

        private void OnLevelRequestedChipsSpawning(SpawnChipsRequest spawnRequest)
        {
            SpawnChipsAsync(spawnRequest.ChipsCount, this.GetCancellationTokenOnDestroy())
                .ContinueWith(spawnRequest.MarkCompleted);
        }

        private async UniTask SpawnChipsAsync(int chipsAmount, CancellationToken ct)
        {
            for (int i = 0; i < chipsAmount; i++)
            {
                var chipIdx = Random.Range(0, _chips.Length);
                var chipId = _chips[chipIdx];
                
                var chipModel = _chipInstantiator.Instantiate(chipId, transform.position, _chipsContainer);
                _levelModel.ChipModels.Add(chipModel);
                
                var randomOffset = Random.insideUnitCircle * _radius;
                chipModel.View.transform.position += new Vector3(randomOffset.x, randomOffset.y, 0);

                if (i < chipsAmount - 1) // last one spawns without delay
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_elementSpawnDelay), cancellationToken: ct);
                    ct.ThrowIfCancellationRequested();
                }
            }
        }
    }
}