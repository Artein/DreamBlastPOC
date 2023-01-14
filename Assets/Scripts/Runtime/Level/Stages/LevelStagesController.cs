using System;
using System.Collections.Generic;
using System.Linq;
using Game.Utils;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using UnityUtils.Invocation;

namespace Game.Level.Stages
{
    [ZenjectBound]
    public class LevelStagesController
    {
        private readonly Dictionary<int, List<ILevelStageGoal>> _stagedGoals = new();
        
        public int CurrentStage { get; private set; }

        public event StageChangedHandler StageChanged;

        // stageOrder — lower is earlier
        [MustUseReturnValue]
        public IDisposable RegisterGoal(int stageOrder, [NotNull] ILevelStageGoal goal)
        {
            if (!_stagedGoals.TryGetValue(stageOrder, out var stageGoals))
            {
                stageGoals = new List<ILevelStageGoal>();
                _stagedGoals.Add(stageOrder, stageGoals);
            }
            
            Assert.IsFalse(stageGoals.Contains(goal));
            stageGoals.Add(goal);
            goal.Finished += OnStageGoalFinished;
            return new DisposableAction(() => goal.Finished -= OnStageGoalFinished);
        }

        private void OnStageGoalFinished(ILevelStageGoal goal)
        {
            goal.Finished -= OnStageGoalFinished;
            UpdateStagesState();
        }

        private void UpdateStagesState()
        {
            if (_stagedGoals.TryGetValue(CurrentStage, out var stageGoals))
            {
                var isStageFinished = stageGoals.All(goal => goal.IsFinished);
                if (isStageFinished)
                {
                    CurrentStage++;
                    StageChanged?.Invoke(CurrentStage);
                }
            }
        }

        public delegate void StageChangedHandler(int newLevelStage);
    }
}