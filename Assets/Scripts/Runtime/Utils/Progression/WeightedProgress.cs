using System;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Utils.Progression
{
    // In modern C# can be generic with IAdditionSubtraction<T> constrain
    public class WeightedProgress : IProgressProvider
    {
        private readonly Progress _progress;
        private float _targetWeight;
        private float _weight_BF;

        public float Weight
        {
            get => _weight_BF;
            set
            {
                var clampedValue = math.max(value, 0f);
                if (Mathf.Approximately(clampedValue, _weight_BF))
                {
                    return;
                }

                _weight_BF = clampedValue;
                _progress.Progress01 = _weight_BF / _targetWeight;
            }
        }
        public float Progress01 => _progress.Progress01;
        public event ValueChangeHandler<float> Changed
        {
            add => _progress.Changed += value;
            remove => _progress.Changed += value;
        }

        public WeightedProgress(float targetWeight, bool logProgressChange = false, string logMessagePrefix = null)
        {
            ValidateTargetWeightArgument(targetWeight);
            
            _targetWeight = targetWeight;
            _progress = new Progress(logProgressChange, logMessagePrefix);
        }

        public void Reset()
        {
            _progress.Reset();
            _weight_BF = 0f;
        }

        public void Reset(float targetWeight)
        {
            ValidateTargetWeightArgument(targetWeight);
            _targetWeight = targetWeight;
            Reset();
        }

        private void ValidateTargetWeightArgument(float targetWeight)
        {
            if (Mathf.Approximately(targetWeight, 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(targetWeight), "Cannot be 0");
            }

            if (targetWeight < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetWeight), "Cannot be negative");
            }
        }
    }
}