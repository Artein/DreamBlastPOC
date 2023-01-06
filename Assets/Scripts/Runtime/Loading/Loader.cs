using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly List<WeightedLoadingTask> _tasks = new();
        private readonly Stopwatch _totalStopwatch = new();
        private readonly Stopwatch _taskStopwatch = new();
        private WeightedProgress _progress;

        [CanBeNull] public IProgressProvider Progress => _progress;

        public event StartingHandler Starting;
        public event FinishingHandler Finishing;
        public event FinishedHandler Finished;
        
        public Loader Enqueue(int weight, ILoadingTask task)
        {
            _tasks.Add(new WeightedLoadingTask { Weight = weight, Task = task });
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
            UnityEngine.Debug.Log($"[Loader] Starting loading of {_tasks.Count} tasks");
            _totalStopwatch.Restart();
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

            _totalStopwatch.Stop();
            UnityEngine.Debug.Log($"[Loader] Loading finished [{_totalStopwatch.ElapsedMilliseconds}ms]");
            return success;
        }

        private async UniTask<bool> ExecuteTaskAsync(WeightedLoadingTask task, CancellationToken cancellationToken)
        {
            task.Task.Progress.Changed += ProgressChanged;

            UnityEngine.Debug.Log($"[Loader] Starting executing '{task.Task}' task");
            _taskStopwatch.Restart();
            var success = await task.Task.ExecuteAsync(cancellationToken);
            _taskStopwatch.Stop();
            UnityEngine.Debug.Log($"[Loader] Ended execution of '{task.Task}' task (success={success})[{_taskStopwatch.ElapsedMilliseconds}ms]");
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
            // TODO GC: Cache and use Locker.Reset()
            var locker = new Locker(false);
            Starting?.Invoke(locker);

            await UniTask.WaitWhile(() => locker.IsLocked, cancellationToken: cancellationToken);
        }

        private async UniTask WaitLoadingFinishingAsync(CancellationToken cancellationToken)
        {
            // TODO GC: Cache and use Locker.Reset()
            var locker = new Locker(false);
            Finishing?.Invoke(locker);

            await UniTask.WaitWhile(() => locker.IsLocked, cancellationToken: cancellationToken);
        }

        public static float CalculateTasksWeight(IReadOnlyList<WeightedLoadingTask> tasks)
        {
            float tasksWeight = 0f;
            for (int i = 0; i < tasks.Count; i += 1)
            {
                var task = tasks[i];
                tasksWeight += task.Weight;
            }

            return tasksWeight;
        }
        
        public delegate void StartingHandler(ILocker startLocker);
        public delegate void FinishingHandler(ILocker finishLocker);
        public delegate void FinishedHandler(bool success);
    }
}