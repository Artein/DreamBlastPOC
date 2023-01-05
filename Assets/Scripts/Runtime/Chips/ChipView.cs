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
        
        [Inject] private ChipModel _chipModel;

        private void OnEnable()
        {
            _chipModel.Destroying += OnChipDestroying;
        }

        private void OnChipDestroying(ChipModel _, IDeferredInvocation destroyDI)
        {
            if (_destroyPlayer == null)
            {
                return;
            }

            var handle = destroyDI.LockInvocation();
            _destroyPlayer.PlayFeedbacksTask(transform.position)
                .AsUniTask()
                .ContinueWith(() => { handle.Dispose(); })
                .AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }
    }
}