using System.ComponentModel;
using Game.Utils;
using UnityEngine;
using UnityUtils.Invocation;
using Zenject;

namespace Game.Chips
{
    [ZenjectBound]
    public class ChipSizeController : IInitializable
    {
        [Inject] private ChipsOptions _chipsOptions;
        [Inject] private ChipModel _chipModel;

        void IInitializable.Initialize()
        {
            UpdateSize();
            _chipsOptions.PropertyChanged += OnChipsOptionsPropertyChanged;
            _chipModel.Destroying += OnChipModelDestroying;
        }

        private void OnChipModelDestroying(ChipModel chipModel, IDeferredInvocation destroyDI)
        {
            _chipModel.Destroying -= OnChipModelDestroying;
            _chipsOptions.PropertyChanged -= OnChipsOptionsPropertyChanged;
        }

        private void OnChipsOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_chipsOptions.ChipsSize))
            {
                UpdateSize();
            }
        }

        private void UpdateSize()
        {
            _chipModel.View.transform.localScale = new Vector3(_chipsOptions.ChipsSize, _chipsOptions.ChipsSize);
        }
    }
}