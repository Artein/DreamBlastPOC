using System;
using System.ComponentModel;
using Game.Utils;
using UnityEngine;
using Zenject;

namespace Game.Level
{
    [ZenjectBound]
    public class LevelConfigProxy : ILevelConfig, IInitializable, IDisposable
    {
        public int TotalChipsAmount => _options.OverrideLevelConfig ? _options.TotalChipsAmount : _config.TotalChipsAmount;
        public float ChipsAmountCheckInterval => _options.OverrideLevelConfig ? _options.ChipsAmountCheckInterval : _config.ChipsAmountCheckInterval;
        public GameObject LevelTopologyPrefab => _config.LevelTopologyPrefab;

        [Inject] private LevelConfig _config;
        [Inject] private LevelOptions _options;
        
        void IInitializable.Initialize()
        {
            _options.PropertyChanged += OnOptionsPropertyChanged;
        }

        void IDisposable.Dispose()
        {
            _options.PropertyChanged -= OnOptionsPropertyChanged;
        }

        private void OnOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO: Update all listeners?
        }
    }
}