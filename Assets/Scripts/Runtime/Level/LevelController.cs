using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Chips;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Level
{
    [UsedImplicitly]
    public class LevelController : IInitializable
    {
        [Inject] private LevelModel _levelModel;
        [Inject] private ILevelConfig _levelConfig;
        [Inject] private IInstantiator _instantiator;
        [Inject(Id = InjectionIds.Transform.LevelContainer)] private Transform _levelContainer;
        [Inject] private CancellationTokenSource _lifetimeCTS;
        private readonly List<SpawnChipsRequest> _activeSpawnChipRequests = new();
        private int ChipsRequestedToSpawn => _activeSpawnChipRequests.Select(r => r.ChipsCount).Sum();

        public event Action<SpawnChipsRequest> ChipsSpawningRequested;
        
        void IInitializable.Initialize()
        {
            _instantiator.InstantiatePrefab(_levelConfig.LevelTopologyPrefab, _levelContainer);
            
            SpawnLevelChipsAsync().Forget();
        }

        private async UniTask SpawnLevelChipsAsync()
        {
            while (!_lifetimeCTS.IsCancellationRequested)
            {
                var missingChipsCount = _levelConfig.TotalChipsAmount - _levelModel.ChipModels.Count - ChipsRequestedToSpawn;
                if (missingChipsCount > 0)
                {
                    var spawnRequest = new SpawnChipsRequest(missingChipsCount);
                    _activeSpawnChipRequests.Add(spawnRequest);
                    spawnRequest.Completed += OnChipsSpawnRequestCompleted;
                    
                    ChipsSpawningRequested?.Invoke(spawnRequest);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_levelConfig.ChipsAmountCheckInterval), cancellationToken: _lifetimeCTS.Token);
            }
        }

        private void OnChipsSpawnRequestCompleted(SpawnChipsRequest request)
        {
            request.Completed -= OnChipsSpawnRequestCompleted;
            _activeSpawnChipRequests.Remove(request);
        }
    }
}