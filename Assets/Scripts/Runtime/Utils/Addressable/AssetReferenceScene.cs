using System;
using System.Linq;
using Eflatun.SceneReference;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Game.Utils.Addressable
{
    // based on Eflatun.SceneReference
    [Serializable]
    public class AssetReferenceScene : AssetReference
    {
        // GUID hex of an invalid asset contains all zeros. A GUID hex has 32 chars.
        private const string AllZeroGuidHex = "00000000000000000000000000000000";
        
        /// <summary>
        /// Construct a new AssetReference object.
        /// </summary>
        /// <param name="guid">The guid of the asset.</param>
        public AssetReferenceScene(string guid) : base(guid)
        {
        }
        
        public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        public string Path
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException($"AssetReferenceScene has no value");
                }

                if (!SceneGuidToPathMapProvider.SceneGuidToPathMap.TryGetValue(AssetGUID, out var scenePath))
                {
                    throw new InvalidOperationException($"Didn't find scene (guid: {AssetGUID}) in SceneGuidToPathMap");
                }

                return scenePath;
            }
        }

        /// <summary>
        /// Is this <see cref="SceneReference"/> assigned something?
        /// </summary>
        /// <remarks>
        /// Only check this property if you need partial validations, as this property alone does not communicate whether this <see cref="SceneReference"/> is absolutely safe to use.<p/>
        /// If you only need to check if it is completely safe to use a <see cref="SceneReference"/> without knowing where exactly the problem is, then only check <see cref="IsSafeToUse"/> instead. Checking only <see cref="IsSafeToUse"/> is sufficient for the majority of the use cases.
        /// </remarks>
        /// <seealso cref="IsInSceneGuidToPathMap"/>
        /// <seealso cref="IsInBuildAndEnabled"/>
        /// <seealso cref="IsSafeToUse"/>
        public bool HasValue
        {
            get
            {
                if (!IsValidGuidHex(AssetGUID))
                {
                    // internal exceptions should not be documented as part of the public API
                    throw new InvalidOperationException($"AssetReferenceScene GUID is not valid");
                }

                return AssetGUID != AllZeroGuidHex;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Type-specific override of parent editorAsset.  Used by the editor to represent the asset referenced.
        /// </summary>
        public new UnityEditor.SceneAsset editorAsset => (UnityEditor.SceneAsset)base.editorAsset;
#endif

        /// <inheritdoc/>
        public override bool ValidateAsset(Object obj)
        {
#if UNITY_EDITOR
            var type = obj.GetType();
            return typeof(UnityEditor.SceneAsset).IsAssignableFrom(type);
#else
        return false;
#endif
        }

        /// <inheritdoc/>
        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var type = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(path);
            return typeof(UnityEditor.SceneAsset).IsAssignableFrom(type);
#else
        return false;
#endif
        }

        public static bool IsValidGuidHex(string guidHex) => guidHex.Length == 32 && guidHex.ToUpper().All("0123456789ABCDEF".Contains);
    }
}