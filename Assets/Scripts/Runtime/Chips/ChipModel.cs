using System;
using Game.Chips.Activation;
using Game.Utils;
using JetBrains.Annotations;
using UnityUtils.Invocation;
using Object = UnityEngine.Object;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipModel
    {
        public enum ChipState : byte
        {
            Default,
            Activating,
            Destroying,
            Destroyed,
        }
        
        public ChipId ChipId { get; }
        public ChipView View { get; set; }
        public IChipActivationExecutor ActivationExecutor { get; }
        public ReactiveProperty<ChipState> State { get; } = new();

        public event DestroyingHandle Destroying;
        public event Action Destroyed;

        public ChipModel(ChipId chipId, IChipActivationExecutor activationExecutor)
        {
            ChipId = chipId;
            ActivationExecutor = activationExecutor;
        }

        public void Activate()
        {
            State.Value = ChipState.Activating;
        }

        public void Destroy()
        {
            State.Value = ChipState.Destroying;
            
            using var destroyDI = new DeferredInvocation(() =>
            {
                Object.Destroy(View.gameObject);
                Destroyed?.Invoke();
                
                View = null; // TODO: Actually would be better to NULL all the values
                State.Value = ChipState.Destroyed;
            });
            Destroying?.Invoke(this, destroyDI);
        }

        public delegate void DestroyingHandle(ChipModel chipModel, IDeferredInvocation destroyDI);
    }
}