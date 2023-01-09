using System.Threading;
using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Loading.Tasks
{
    public class UnloadBuiltInSceneTask : BaseLoadingTask
    {
        private readonly SceneReference _sceneReference;

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
            var unloadSceneOperation = SceneManager.UnloadSceneAsync(_sceneReference.Name);
            SetProgress01(unloadSceneOperation.progress * 0.5f);
            
            while (!unloadSceneOperation.isDone)
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress01(unloadSceneOperation.progress * 0.5f);
            }
            
            var unloadOperation = Resources.UnloadUnusedAssets();
            SetProgress01(0.5f + unloadOperation.progress * 0.5f);
            
            while (!unloadOperation.isDone)
            {
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
                
                SetProgress01(0.5f + unloadOperation.progress * 0.5f);
            }
            
            SetProgress01(1f);

            return true;
        }
    }
}