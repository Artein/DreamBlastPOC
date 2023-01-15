using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Loading.Tasks;
using Game.Utils;
using Game.Utils.Progression;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using UnityUtils.State.Locking;
using Debug = UnityEngine.Debug;

namespace Game.Loading
{
    [ZenjectBound]
    public class Loader
    {
        private readonly List<WeightedLoadingTask> _tasks = new();
        private readonly Stopwatch _totalStopwatch = new();
        private readonly Stopwatch _taskStopwatch = new();
        private readonly Locker _loadingStartLocker = new(false);
        private readonly Locker _loadingFinishLocker = new(false);
        private WeightedProgress _progress;

        [CanBeNull] public IProgressProvider Progress => _progress;
        public bool IsLoading { get; private set; }
        public bool LoggingEnabled { get; set; }

        public event StartingHandler Starting;
        public event FinishingHandler Finishing;
        public event FinishedHandler Finished;
        
        public Loader Enqueue(int weight, ILoadingTask task)
        {
            _tasks.Add(new WeightedLoadingTask { Weight = weight, Task = task });
            return this;
        }

        public Loader Enqueue(WeightedLoadingTask task)
        {
            _tasks.Add(task);
            return this;
        }

        public void Reset()
        {
            _tasks.Clear();
            _progress = null;
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

        public async UniTask<bool> StartAsync(CancellationToken cancellationToken, bool resetOnFinish = true)
        {
            Assert.IsFalse(IsLoading);
            Assert.IsTrue(_tasks.Count > 0);
            Log($"Starting loading of {_tasks.Count} tasks");
            _totalStopwatch.Restart();

            IsLoading = true;
            var success = false;
            var tasksWeight = CalculateTasksWeight(_tasks);
            InitializeProgress(tasksWeight);

            try
            {
                await WaitLoadingStartingAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

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
            }
            finally
            {
                _totalStopwatch.Stop();
                IsLoading = false;
                Log($"Loading finished. Success={success} [{_totalStopwatch.ElapsedMilliseconds}ms]");
            }

            return success;
        }

        private async UniTask<bool> ExecuteTaskAsync(WeightedLoadingTask task, CancellationToken cancellationToken)
        {
            task.Task.Progress.Changed += ProgressChanged;

            Log($"Starting executing '{task.Task}' task");
            _taskStopwatch.Restart();
            var success = await task.Task.ExecuteAsync(cancellationToken);
            _taskStopwatch.Stop();
            Log($"Ended execution of '{task.Task}' task (success={success})[{_taskStopwatch.ElapsedMilliseconds}ms]");
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
            Log("Begin waiting loading Start");
            _loadingStartLocker.Reset();
            Starting?.Invoke(_loadingStartLocker);

            await UniTask.WaitWhile(() => _loadingStartLocker.IsLocked, cancellationToken: cancellationToken);
            Log("Ended waiting loading Start");
        }

        private async UniTask WaitLoadingFinishingAsync(CancellationToken cancellationToken)
        {
            Log("Begin waiting loading Finish");
            _loadingFinishLocker.Reset();
            Finishing?.Invoke(_loadingFinishLocker);

            await UniTask.WaitWhile(() => _loadingFinishLocker.IsLocked, cancellationToken: cancellationToken);
            Log("Ended waiting loading Finish");
        }

        private void InitializeProgress(float tasksWeight)
        {
            if (_progress != null)
            {
                _progress.Reset(tasksWeight);
            }
            else
            {
                _progress = new WeightedProgress(tasksWeight, LoggingEnabled, nameof(Loader));
            }
        }

        private void Log(string message)
        {
            if (LoggingEnabled)
            {
                Debug.unityLogger.Log(nameof(Loader), message);
            }
        }

        public delegate void StartingHandler(ILocker startLocker);
        public delegate void FinishingHandler(ILocker finishLocker);
        public delegate void FinishedHandler(bool success);
    }
}