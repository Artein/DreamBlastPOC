using Game.Chips;
using UnityEngine;
using Zenject;

namespace Game.Helpers
{
    [DisallowMultipleComponent]
    public class DestroyOnMatchPerformedBehaviour : MonoBehaviour
    {
        [SerializeField, Min(2)] private int _matchSize = 2;

        [Inject] private SignalBus _signalBus;

        private void OnEnable()
        {
            _signalBus.Subscribe<ChipsMatchPerformedSignal>(OnChipsMatchPerformed);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<ChipsMatchPerformedSignal>(OnChipsMatchPerformed);
        }

        private void OnChipsMatchPerformed(ChipsMatchPerformedSignal chipsMatchSignal)
        {
            if (chipsMatchSignal.MatchSize >= _matchSize)
            {
                Destroy(gameObject);
            }
        }
    }
}