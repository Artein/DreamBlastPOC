using Game.Chips.Activation;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game.Chips
{
    [UsedImplicitly]
    public class ChipModel
    {
        public ChipId ChipId { get; }
        public GameObject View { get; set; }
        public IChipActivationExecutor ActivationExecutor { get; }
        
        [Inject(Id = InjectionIds.Value.IgnoreRaycastsLayer)]
        private int _ignoreRaycastsLayer;

        public ChipModel(ChipId chipId, IChipActivationExecutor activationExecutor)
        {
            ChipId = chipId;
            ActivationExecutor = activationExecutor;
        }

        public void Destroy()
        {
            View.gameObject.layer = _ignoreRaycastsLayer;
            Object.Destroy(View.gameObject);
        }
    }
}