using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;
using UnityEngine.Assertions;

namespace Game.Loading.Tasks
{
    // Tasks passed to CompositeLoadingTask invokes in parallel to each other
    public class CompositeLoadingTask : ILoadingTask
    {
        private readonly List<WeightedLoadingTask> _tasks;
        private readonly WeightedProgress _progress;
        
        public bool IsExecuting { get; private set; }
        public IProgressProvider Progress => _progress;

        public CompositeLoadingTask(List<WeightedLoadingTask> tasks)
        {
            _tasks = tasks;
            var tasksWeight = Loader.CalculateTasksWeight(tasks);
            _progress = new WeightedProgress(tasksWeight);
        }
        
        public async UniTask<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            _progress.Reset();
            Assert.IsFalse(IsExecuting);
            IsExecuting = true;

            var executingTasks = new List<UniTask<bool>>(_tasks.Count);

            for (int i = 0; i < _tasks.Count; i += 1)
            {
                var task = _tasks[i];
                task.Task.Progress.Changed += TaskProgressChanged;

                var executingTask = task.Task.ExecuteAsync(cancellationToken);
                executingTasks.Add(executingTask);
            }

            // TODO: Handle a task fail state. Is whole composite should fail as well?
            await UniTask.WhenAll(executingTasks);

            for (int i = 0; i < _tasks.Count; i += 1)
            {
                var task = _tasks[i];
                task.Task.Progress.Changed -= TaskProgressChanged;
            }

            IsExecuting = false;

            return true;
        }

        private void TaskProgressChanged(float value, float prevValue)
        {
            float progressedWeight = 0f;
            for (int i = 0; i < _tasks.Count; i += 1)
            {
                var task = _tasks[i];
                progressedWeight += task.Weight * task.Task.Progress.Progress01;
            }

            _progress.Weight = progressedWeight;
        }
    }
}