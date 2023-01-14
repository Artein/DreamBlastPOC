using System;
using System.Linq;
using Game.Input;
using Game.Level;
using Game.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Game.Chips.Activation
{
    [ZenjectBound]
    public class ChipByUserInputActivator : IInitializable, IDisposable
    {
        [Inject] private GameObjectInputNotifier _layeredInputNotifier;
        [Inject] private LevelModel _levelModel;

        [Inject(Id = InjectionIds.Int.ChipsLayer)]
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
            var touchedChipView = gameobject.GetComponentInParent<ChipView>();
            Assert.IsNotNull(touchedChipView);
            Debug.unityLogger.Log(nameof(ChipByUserInputActivator), $"Touched chip '{gameobject.name}'", touchedChipView);
            Assert.IsTrue(_levelModel.ChipModels.Any(chipModel => chipModel.View == touchedChipView));
            var touchedChipModel = _levelModel.ChipModels.First(chipModel => chipModel.View == touchedChipView);
            // TODO: ChipModel might be a Facade with all possible interactions about chip, or extract ActionExecutor into outer container and call from it
            bool isActivated = touchedChipModel.ActivationExecutor.TryActivate(touchedChipModel);
            if (isActivated)
            {
                touchedChipModel.Activate();
            }
        }
    }
}