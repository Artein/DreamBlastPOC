using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SRDebugger.Services;
using UnityEngine;
using Zenject;

namespace Game.Physics
{
    [UsedImplicitly]
    public class PhysicsOptions : IInitializable, IDisposable, INotifyPropertyChanged
    {
        [Inject] private IDebugService _debugService;
        [Inject] private PhysicsSimulationExecutor _physicsSimulationExecutor;

        [Category("Physics")]
        public SimulationMode2D PhysicsMode
        {
            get => Physics2D.simulationMode;
            set
            {
                if (Physics2D.simulationMode != value)
                {
                    Physics2D.simulationMode = value;
                    _physicsSimulationExecutor.enabled = value == SimulationMode2D.Script;
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