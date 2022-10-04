using System.Collections.Generic;
using System.Linq;
using Game.Level;
using JetBrains.Annotations;
using Unity.Profiling;
using UnityEngine;
using Zenject;

namespace Game.Chips.Activation
{
    [UsedImplicitly]
    public class ColoredChipsBlobActivationExecutor : IChipActivationExecutor
    {
        [Inject] private LevelModel _levelModel;
        [Inject(Id = InjectionIds.Transform.ChipsContainer)] private Transform _chipsContainer;
        [Inject] private ChipInstantiator _chipInstantiator;
        [Inject] private ColoredChipsActivationConfig _activationConfig;
        private ProfilerMarker _tryActivateProfilerMarker = new($"{nameof(ColoredChipsBlobActivationExecutor)}.{nameof(TryActivate)}");

        private readonly List<ChipModel> _nearbySimilarChips = new();
        private readonly List<ChipModel> _nearbyChipsToCheck = new();
        
        // TODO Performance: Optimize me
        public bool TryActivate(ChipModel pivotChipModel)
        {
            using var profileScopeHandle = _tryActivateProfilerMarker.Auto();
            var allSimilarChips = _levelModel.ChipModels.Where(cm => cm.ChipId == pivotChipModel.ChipId).ToList();
            var allSimilarChipsPositions = allSimilarChips.Select(chip => chip.View.transform.position).ToList();

            _nearbySimilarChips.Clear();
            _nearbySimilarChips.Add(pivotChipModel);
            _nearbyChipsToCheck.Clear();
            _nearbyChipsToCheck.Add(pivotChipModel);
            
            while (_nearbyChipsToCheck.Count > 0)
            {
                var chipToCheck = _nearbyChipsToCheck[^1];
                var position = chipToCheck.View.transform.position;
                for (int i = 0; i < allSimilarChipsPositions.Count; i++)
                {
                    var similarChipPosition = allSimilarChipsPositions[i];
                    if (Vector3.Distance(position, similarChipPosition) <= _activationConfig.ChipMatchRadius && position != similarChipPosition)
                    {
                        var possibleChip = allSimilarChips[i];
                        
                        if (!_nearbySimilarChips.Contains(possibleChip))
                        {
                            _nearbySimilarChips.Add(possibleChip);
                            if (!_nearbyChipsToCheck.Contains(possibleChip))
                            {
                                _nearbyChipsToCheck.Add(possibleChip);
                            }
                        }
                    }
                }

                _nearbyChipsToCheck.Remove(chipToCheck);
            }

            if (_nearbySimilarChips.Count >= _activationConfig.SimilarColorMinMatchSize)
            {
                PerformMatch(_nearbySimilarChips, pivotChipModel);
                return true;
            }

            return false;
        }

        private void PerformMatch(IReadOnlyList<ChipModel> chips, ChipModel pivotChip)
        {
            for (int i = 0; i < chips.Count; i++)
            {
                var chip = chips[i];
                _levelModel.ChipModels.Remove(chip);
                chip.Destroy();
            }

            if (_activationConfig.TryGetChipToCreateAfterMatch(chips.Count, out var chipIdToCreate))
            {
                var newChip = _chipInstantiator.Instantiate(chipIdToCreate, pivotChip.View.transform.position, _chipsContainer);
                _levelModel.ChipModels.Add(newChip);
            }
        }
    }
}