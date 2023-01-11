using System.Collections.Generic;
using Game.Utils.Addressable;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Game.Loading
{
    public class AddressableScenesStorage
    {
        private readonly List<(AssetReferenceScene, AsyncOperationHandle<SceneInstance>)> _sceneLoadOperations = new();
        
        public void AddLoadOperation(AssetReferenceScene sceneRef, AsyncOperationHandle<SceneInstance> operation)
        {
            Assert.IsFalse(Contains(sceneRef));
            _sceneLoadOperations.Add((sceneRef, operation));
        }

        public AsyncOperationHandle<SceneInstance>? TakeLoadOperation(AssetReferenceScene takingSceneRef)
        {
            for (int i = 0; i < _sceneLoadOperations.Count; i += 1)
            {
                var (sceneRef, loadOperation) = _sceneLoadOperations[i];
                if (takingSceneRef.SceneName == sceneRef.SceneName)
                {
                    _sceneLoadOperations.RemoveAt(i);
                    return loadOperation;
                }
            }

            return null;
        }

        public bool Contains(AssetReferenceScene sceneRef)
        {
            for (int i = 0; i < _sceneLoadOperations.Count; i += 1)
            {
                var (storingSceneRef, _) = _sceneLoadOperations[i];
                if (storingSceneRef.SceneName == sceneRef.SceneName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}