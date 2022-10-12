using System;
using System.Collections.Generic;
using Game.Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game.Level.Stages
{
    public class LevelStageDestroyGoalBehaviour : MonoBehaviour, ILevelStageGoal
    {
        [SerializeField, Min(0)] private int _stageOrder;
        [SerializeField] private List<GameObject> _targets;

        [Inject] private LevelStagesController _levelStagesController;
        private IDisposable _goalRegistrationHandle;
        private List<DestroyNotifier> _targetNotifiers;

        public bool IsFinished { get; private set; }

        private void Awake()
        {
            _targetNotifiers = new List<DestroyNotifier>(_targets.Count);
            foreach (var target in _targets)
            {
                if (!target.TryGetComponent<DestroyNotifier>(out var notifier))
                {
                    notifier = target.AddComponent<DestroyNotifier>();
                }
                
                _targetNotifiers.Add(notifier);
            }
        }

        private void OnEnable()
        {
            _goalRegistrationHandle = _levelStagesController.RegisterGoal(_stageOrder, this);
            
            foreach (var notifier in _targetNotifiers)
            {
                notifier.Destroying += OnTargetDestroying;
            }
        }

        private void OnDisable()
        {
            foreach (var notifier in _targetNotifiers)
            {
                notifier.Destroying -= OnTargetDestroying;
            }
            
            _goalRegistrationHandle?.Dispose();
            _goalRegistrationHandle = null;
        }

        private void OnTargetDestroying(DestroyNotifier targetNotifier)
        {
            targetNotifier.Destroying -= OnTargetDestroying;
            _targets.Remove(targetNotifier.gameObject);
            UpdateGoalState();
        }

        private void UpdateGoalState()
        {
            Assert.IsFalse(IsFinished);
            IsFinished = _targets.Count == 0;
            if (IsFinished)
            {
                Finished?.Invoke(this);
            }
        }

        public event Action<ILevelStageGoal> Finished;
    }
}