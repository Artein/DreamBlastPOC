using System;
using Game.Input;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game
{
    [UsedImplicitly]
    public class ChipByUserInputActivator : IInitializable, IDisposable
    {
        [Inject] private GameObjectInputNotifier _layeredInputNotifier;

        [Inject(Id = InjectionIds.Value.ChipsLayer)]
        private int _chipsLayer;

        [Inject(Id = InjectionIds.Value.IgnoreRaycastsLayer)] private int _ignoreRaycastLayer;

        private IDisposable _subscriptionHandle;

        void IInitializable.Initialize()
        {
            Assert.IsNull(_subscriptionHandle);
            _subscriptionHandle = _layeredInputNotifier.Subscribe(_chipsLayer, OnChipTouched);
        }

        void IDisposable.Dispose()
        {
            _subscriptionHandle?.Dispose();
            _subscriptionHandle = null;
        }

        private void OnChipTouched(GameObject gameobject)
        {
            UnityEngine.Debug.Log($"Touched chip '{gameobject.name}'", gameobject);
            gameobject.layer = _ignoreRaycastLayer;
        }
    }
}