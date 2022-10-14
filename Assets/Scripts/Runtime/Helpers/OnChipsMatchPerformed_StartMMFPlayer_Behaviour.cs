using Game.Chips;
using MoreMountains.Feedbacks;
using UnityEngine;
using Zenject;

namespace Game.Helpers
{
    [RequireComponent(typeof(MMF_Player))]
    public class OnChipsMatchPerformed_StartMMFPlayer_Behaviour : MonoBehaviour
    {
        [SerializeField, Min(2)] private int _matchSize;
        [SerializeField, Range(0, 10)] private float _feedbackIntensityMultiplier = 1f;

        [Inject] private SignalBus _signalBus;
        private MMF_Player _mmfPlayer;

        private void Awake()
        {
            _mmfPlayer = GetComponent<MMF_Player>();
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<ChipsMatchPerformedSignal>(OnChipsMatchPerformedSignalReceived);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<ChipsMatchPerformedSignal>(OnChipsMatchPerformedSignalReceived);
        }

        private void OnChipsMatchPerformedSignalReceived(ChipsMatchPerformedSignal signal)
        {
            if (signal.MatchSize >= _matchSize)
            {
                _mmfPlayer.FeedbacksIntensity = (signal.MatchSize - _matchSize) * _feedbackIntensityMultiplier + 1; // Plus 1 to play normal feedback at exact match
                _mmfPlayer.PlayFeedbacks();
            }
        }
    }
}