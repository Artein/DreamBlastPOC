using Game.Level;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Game.Chips
{
    [DisallowMultipleComponent]
    public class ChipPlaceholder : MonoBehaviour
    {
        [SerializeField, Required] private ChipId _chipId;

        [Inject] private LevelModel _levelModel;
        [Inject] private ChipInstantiator _chipInstantiator;
        [Inject(Id = InjectionIds.Transform.ChipsContainer)] private Transform _chipsContainer;

        private void Start()
        {
            var chipModel = _chipInstantiator.Instantiate(_chipId, transform.position, _chipsContainer);
            chipModel.View.name += "_FromPlaceholder";
            _levelModel.ChipModels.Add(chipModel);
            Destroy(gameObject);
        }
    }
}