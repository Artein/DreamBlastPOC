using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SRDebugger;
using SRDebugger.Services;
using Zenject;

namespace Game.Options
{
    [UsedImplicitly]
    public class LevelOptions : IInitializable, IDisposable, INotifyPropertyChanged
    {
        [Inject] private IDebugService _debugService;
        private bool _overrideLevelConfig;
        private int _totalChipsAmount;
        private float _chipsAmountCheckInterval;

        public bool OverrideLevelConfig
        {
            get => _overrideLevelConfig;
            set
            {
                if (_overrideLevelConfig != value)
                {
                    _overrideLevelConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        [NumberRange(0, 99999), Increment(20d)]
        public int TotalChipsAmount
        {
            get => _totalChipsAmount;
            set
            {
                if (_totalChipsAmount != value)
                {
                    _totalChipsAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        [NumberRange(0d, 10d), Increment(0.1d)]
        public float ChipsAmountCheckInterval
        {
            get => _chipsAmountCheckInterval;
            set
            {
                if (_chipsAmountCheckInterval != value)
                {
                    _chipsAmountCheckInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        void IInitializable.Initialize()
        {
            _debugService.AddOptionContainer(this);
        }

        void IDisposable.Dispose()
        {
            _debugService.RemoveOptionContainer(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}