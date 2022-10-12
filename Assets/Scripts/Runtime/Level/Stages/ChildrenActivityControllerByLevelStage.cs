using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Level.Stages
{
    [DisallowMultipleComponent]
    public class ChildrenActivityControllerByLevelStage : MonoBehaviour
    {
        [SerializeField, Min(0)] private List<int> _stageOrders;
        
        [Inject] private LevelStagesController _levelStagesController;
        
        private void Awake()
        {
            UpdateActivity(_levelStagesController.CurrentStage);
            _levelStagesController.StageChanged += OnLevelStageChanged;
        }

        private void OnDestroy()
        {
            _levelStagesController.StageChanged -= OnLevelStageChanged;
        }

        private void OnLevelStageChanged(int newLevelStage)
        {
            UpdateActivity(newLevelStage);
        }

        private void UpdateActivity(int currentLevelStage)
        {
            var isActive = _stageOrders.Contains(currentLevelStage);
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.gameObject.SetActive(isActive);
            }
        }
    }
}