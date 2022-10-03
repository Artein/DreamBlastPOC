using System.ComponentModel;
using Game.Options;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipSizeController : IInitializable
    {
        [Inject] private LevelOptions _levelOptions;
        [Inject] private ChipModel _chipModel;

        void IInitializable.Initialize()
        {
            UpdateSize();
            _levelOptions.PropertyChanged += OnLevelOptionsPropertyChanged;
            _chipModel.Destroying += OnChipModelDestroying;
        }

        private void OnChipModelDestroying(ChipModel chipModel)
        {
            _chipModel.Destroying -= OnChipModelDestroying;
            _levelOptions.PropertyChanged -= OnLevelOptionsPropertyChanged;
        }

        private void OnLevelOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_levelOptions.ChipsSize))
            {
                UpdateSize();
            }
        }

        private void UpdateSize()
        {
            _chipModel.View.transform.localScale = new Vector3(_levelOptions.ChipsSize, _levelOptions.ChipsSize);
        }
    }
}