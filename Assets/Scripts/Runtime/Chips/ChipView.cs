using Cysharp.Threading.Tasks;
using Game.Utils;
using MoreMountains.Feedbacks;
using UnityEngine;
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

            var handle = destroyDI.Lock();
            _destroyPlayer.PlayFeedbacksTask(transform.position)
                .AsUniTask()
                .ContinueWith(() => { handle.Dispose(); })
                .AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
        }
    }
}