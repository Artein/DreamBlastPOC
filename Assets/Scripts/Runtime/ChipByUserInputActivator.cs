using System;
using System.Linq;
using Game.Input;
using Game.Level;
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
        [Inject] private LevelModel _levelModel;

        [Inject(Id = InjectionIds.Value.ChipsLayer)]
        private int _chipsLayer;

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
            var touchedChipModel = _levelModel.ChipModels.First(chipModel => chipModel.View == gameobject);
            bool isActivated = touchedChipModel.ActivationExecutor.TryActivate(touchedChipModel);
        }
    }
}