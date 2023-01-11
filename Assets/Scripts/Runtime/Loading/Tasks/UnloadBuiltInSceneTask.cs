using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using Game.Utils.Progression;
using UnityEngine;
using UnityEngine.SceneManagement;
using Progress = Game.Utils.Progression.Progress;

namespace Game.Loading.Tasks
{
    public class UnloadBuiltInSceneTask : BaseLoadingTask
    {
        private readonly SceneReference _sceneReference;
        private readonly Progress _progress = new();

        public override IProgressProvider Progress => _progress;

        public UnloadBuiltInSceneTask(SceneReference sceneReference)
        {
            _sceneReference = sceneReference;
        }

        public override string ToString()
        {
            return $"{nameof(UnloadBuiltInSceneTask)}({_sceneReference.Name})";
        }

        protected override async UniTask<bool> ExecuteAsync_Implementation(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var unloadSceneOperation = SceneManager.UnloadSceneAsync(_sceneReference.Name);
            _progress.Progress01 = unloadSceneOperation.progress * 0.5f;
            
            while (!unloadSceneOperation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                _progress.Progress01 = unloadSceneOperation.progress * 0.5f;
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            var unloadOperation = Resources.UnloadUnusedAssets();
            _progress.Progress01 = 0.5f + unloadOperation.progress * 0.5f;
            
            while (!unloadOperation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                _progress.Progress01 = 0.5f + unloadOperation.progress * 0.5f;
            }
            
            _progress.Progress01 = 1f;
            return true;
        }
    }
}