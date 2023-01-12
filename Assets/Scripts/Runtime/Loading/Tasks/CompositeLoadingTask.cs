using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Utils.Progression;

namespace Game.Loading.Tasks
{
    // Tasks passed to CompositeLoadingTask invokes in parallel to each other
    public class CompositeLoadingTask : BaseLoadingTask
    {
        private readonly List<WeightedLoadingTask> _tasks;
        private readonly WeightedProgress _progress;
        private readonly bool _failIfAnyTaskFailed;

        public override IProgressProvider Progress => _progress;

        public CompositeLoadingTask(List<WeightedLoadingTask> tasks, bool failIfAnyTaskFailed = false)
        {
            _failIfAnyTaskFailed = failIfAnyTaskFailed;
            _tasks = tasks;
            var tasksWeight = Loader.CalculateTasksWeight(tasks);
            _progress = new WeightedProgress(tasksWeight);
        }

        public override string ToString()
        {
            return $"{nameof(CompositeLoadingTask)} of {_tasks.Count} ({string.Join(", ", _tasks.Select(t => t.Task.ToString()))})";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            _progress.Reset();
            
            var executingTasks = new List<UniTask<bool>>(_tasks.Count);

            for (int i = 0; i < _tasks.Count; i += 1)
            {
                var task = _tasks[i];
                task.Task.Progress.Changed += TaskProgressChanged;

                var executingTask = task.Task.ExecuteAsync(cancellationToken);
                executingTasks.Add(executingTask);
            }

            bool[] results;
            try
            {
                results = await UniTask.WhenAll(executingTasks);
            }
            finally
            {
                for (int i = 0; i < _tasks.Count; i += 1)
                {
                    var task = _tasks[i];
                    task.Task.Progress.Changed -= TaskProgressChanged;
                }
            }

            bool success = true;
            if (_failIfAnyTaskFailed)
            {
                success = results.All(taskSucceeded => taskSucceeded);
            }
            return success;
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