using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Game.Utils;
using JetBrains.Annotations;
using SRDebugger;
using SRDebugger.Services;
using Zenject;

namespace Game.Chips
{
    [ZenjectBound]
    public class ChipsOptions : IInitializable, IDisposable, INotifyPropertyChanged
    {
        [Inject] private IDebugService _debugService;
        private float _chipsSize = 1f;
        private bool _animatedChips;
        
        [Category("Chips"), NumberRange(0.1d, 100d), Increment(0.1d)]
        public float ChipsSize
        {
            get => _chipsSize;
            set
            {
                if (_chipsSize != value)
                {
                    _chipsSize = value;
                    OnPropertyChanged();
                }
            }
        }

        [Category("Chips")]
        public bool AnimatedChips
        {
            get => _animatedChips;
            set
            {
                if (_animatedChips != value)
                {
                    _animatedChips = value;
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