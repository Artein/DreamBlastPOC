using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityUtils.Invocation;
using Zenject;

namespace Game.Chips
{
    public class ChipView : MonoBehaviour
    {
        [SerializeField] private MMF_Player _destroyPlayer;
        
        [Inject(Id = InjectionIds.Int.IgnoreRaycastsLayer)]
        private int _ignoreRaycastsLayer;
        [Inject] private ChipModel _chipModel;

        private void OnEnable()
        {
            _chipModel.Destroying += OnChipDestroying;
        }

        private void OnChipDestroying(ChipModel _, IDeferredInvocation destroyDI)
        {
            gameObject.layer = _ignoreRaycastsLayer;
            
            if (_destroyPlayer != null)
            {
                var handle = destroyDI.LockInvocation();
                _destroyPlayer.PlayFeedbacksTask(transform.position)
                    .AsUniTask()
                    .ContinueWith(() => { handle.Dispose(); })
                    .AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
            }
        }
    }
}