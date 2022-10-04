using System;
using Game.Chips.Activation;
using JetBrains.Annotations;
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

        public event Action<ChipModel> Destroying;

        public ChipModel(ChipId chipId, IChipActivationExecutor activationExecutor)
        {
            ChipId = chipId;
            ActivationExecutor = activationExecutor;
        }

        public void Destroy()
        {
            View.gameObject.layer = _ignoreRaycastsLayer;
            Destroying?.Invoke(this);
            Object.Destroy(View.gameObject);
        }
    }
}