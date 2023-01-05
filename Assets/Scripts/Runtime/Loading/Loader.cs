using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading.Tasks;
using Game.Utils.Locking;
using Game.Utils.Progression;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Game.Loading
{
    [UsedImplicitly]
    public class Loader
    {
        private readonly List<WeightedTask> _tasks = new();
        private WeightedProgress _progress;

        [CanBeNull] public IProgressProvider Progress => _progress;

        public event StartingHandler Starting;
        public event FinishingHandler Finishing;
        public event FinishedHandler Finished;
        
        public Loader Enqueue(int weight, ILoadingTask task)
        {
            _tasks.Add(new WeightedTask { Weight = weight, Task = task });
            return this;
        }

        public void Reset()
        {
            _tasks.Clear();
            _progress = null;
        }

        public async UniTask<bool> StartAsync(CancellationToken cancellationToken, bool resetOnFinish = true)
        {
            Assert.IsTrue(_tasks.Count > 0);
            var tasksWeight = CalculateTasksWeight(_tasks);
            _progress = new WeightedProgress(tasksWeight, true, "[Loading] ");
            
            await WaitLoadingStartingAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var success = false;
            for (int i = 0; i < _tasks.Count; i += 1)
            {
                var task = _tasks[i];
                success = await ExecuteTaskAsync(task, cancellationToken);
                if (!success)
                {
                    break;
                }
            }

            if (success)
            {
                _progress.Weight = tasksWeight;
            }

            await WaitLoadingFinishingAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            Finished?.Invoke(success);
            if (resetOnFinish)
            {
                Reset();
            }

            return success;
        }

        private async UniTask<bool> ExecuteTaskAsync(WeightedTask task, CancellationToken cancellationToken)
        {
            task.Task.Progress.Changed += ProgressChanged;

            var success = await task.Task.ExecuteAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            
            task.Task.Progress.Changed -= ProgressChanged;

            return success;

            void ProgressChanged(float progress01, float prevProgress01)
            {
                var diff = progress01 - prevProgress01;
                _progress.Weight += diff * task.Weight;
            }
        }

        private async UniTask WaitLoadingStartingAsync(CancellationToken cancellationToken)
        {
            var locker = new Locker(false);
            Starting?.Invoke(locker);

            await UniTask.WaitWhile(() => locker.IsLocked, cancellationToken: cancellationToken);
        }

        private async UniTask WaitLoadingFinishingAsync(CancellationToken cancellationToken)
        {
            var locker = new Locker(false);
            Finishing?.Invoke(locker);

            await UniTask.WaitWhile(() => locker.IsLocked, cancellationToken: cancellationToken);
        }

        public static float CalculateTasksWeight(IReadOnlyList<WeightedTask> tasks)
        {
            float tasksWeight = 0f;
            for (int i = 0; i < tasks.Count; i += 1)
            {
                var task = tasks[i];
                tasksWeight += task.Weight;
            }

            return tasksWeight;
        }

        public struct WeightedTask
        {
            public int Weight;
            public ILoadingTask Task;

            public WeightedTask(ILoadingTask task)
            {
                Weight = 1;
                Task = task;
            }
        }
        
        public delegate void StartingHandler(ILocker startLocker);
        public delegate void FinishingHandler(ILocker finishLocker);
        public delegate void FinishedHandler(bool success);
    }
}