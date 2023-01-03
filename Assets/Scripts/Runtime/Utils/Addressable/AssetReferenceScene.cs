using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Utils.Addressable
{
    [Serializable]
    public class AssetReferenceScene : AssetReference
    {
        [SerializeField] private string _sceneName;
 
        public string SceneName => _sceneName;
 
#if UNITY_EDITOR
        public AssetReferenceScene(SceneAsset scene) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene)))
        {
            _sceneName = scene.name;
        }
 
        public override bool ValidateAsset(string path)
        {
            return ValidateAsset(AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
        }
 
        public override bool ValidateAsset(UnityEngine.Object obj)
        {
            return obj != null && obj is SceneAsset;
        }
 
        public override bool SetEditorAsset(UnityEngine.Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }
 
            if (value is SceneAsset scene)
            {
                _sceneName = scene.name;
                return true;
            }
            
            _sceneName = string.Empty;
            return false;
        }
#endif // UNITY_EDITOR
    }
}