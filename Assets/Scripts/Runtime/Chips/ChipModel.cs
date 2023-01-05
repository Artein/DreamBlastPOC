using System;
using Game.Chips.Activation;
using JetBrains.Annotations;
using UnityUtils.Invocation;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipModel
    {
        public ChipId ChipId { get; }
        public ChipView View { get; set; }
        public IChipActivationExecutor ActivationExecutor { get; }
        
        [Inject(Id = InjectionIds.Int.IgnoreRaycastsLayer)]
        private int _ignoreRaycastsLayer;

        public event DestroyingHandle Destroying;
        public event Action Destroyed;

        public ChipModel(ChipId chipId, IChipActivationExecutor activationExecutor)
        {
            ChipId = chipId;
            ActivationExecutor = activationExecutor;
        }

        public void Destroy()
        {
            // TODO: This actually should be ChipDestroyController
            View.gameObject.layer = _ignoreRaycastsLayer;
            
            using var destroyDI = new DeferredInvocation(() =>
            {
                Object.Destroy(View.gameObject);
                Destroyed?.Invoke();
                
                View = null; // TODO: Actually would be better to NULL all the values
            });
            Destroying?.Invoke(this, destroyDI);
        }

        public delegate void DestroyingHandle(ChipModel chipModel, IDeferredInvocation destroyDI);
    }
}