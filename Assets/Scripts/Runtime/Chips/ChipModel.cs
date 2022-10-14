using Game.Chips.Activation;
using Game.Utils;
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

        public event DestroyingHandle Destroying;

        public ChipModel(ChipId chipId, IChipActivationExecutor activationExecutor)
        {
            ChipId = chipId;
            ActivationExecutor = activationExecutor;
        }

        public void Destroy()
        {
            using var destroyDI = new DeferredInvocation(() =>
            {
                View.gameObject.layer = _ignoreRaycastsLayer;
                Object.Destroy(View.gameObject);
                View = null;
            });
            Destroying?.Invoke(this, destroyDI);
        }

        public delegate void DestroyingHandle(ChipModel chipModel, IDeferredInvocation destroyDI);
    }
}