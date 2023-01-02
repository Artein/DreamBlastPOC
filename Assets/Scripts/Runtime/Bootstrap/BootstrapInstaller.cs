using Game.Utils.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Bootstrap
{
    [DisallowMultipleComponent]
    public class BootstrapInstaller : MonoInstaller<BootstrapInstaller>
    {
        [SerializeField] private AssetReferenceScene _targetSceneRef;
        [SerializeField] private AssetLabelReference _preLoadAssetLabel;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BootstrapExecutor>().AsSingle().WithArguments(_targetSceneRef, _preLoadAssetLabel).NonLazy();
        }

        private void OnValidate()
        {
            _targetSceneRef.RuntimeKeyIsValid();
            _preLoadAssetLabel.RuntimeKeyIsValid();
        }
    }
}