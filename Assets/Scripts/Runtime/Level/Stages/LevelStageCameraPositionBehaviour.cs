using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Game.Level.Stages
{
    [DisallowMultipleComponent]
    public class LevelStageCameraPositionBehaviour : MonoBehaviour
    {
        [SerializeField, Min(0f)] private int _stageOrder;
        [SerializeField, Min(0.1f)] private float _transitionDuration;
        
        [Inject] private LevelStagesController _levelStagesController;
        [Inject(Id = InjectionIds.Transform.CameraRig)] private Transform _cameraRig;
        
        private CancellationTokenSource _movementCTS;

        private void OnEnable()
        {
            _levelStagesController.StageChanged += OnLevelStageChanged;
            
            // Camera initial transition from (0,0,0)
            OnLevelStageChanged(_levelStagesController.CurrentStage);
        }

        private void OnDisable()
        {
            _levelStagesController.StageChanged -= OnLevelStageChanged;
        }

        private void OnLevelStageChanged(int newLevelStage)
        {
            _movementCTS?.Cancel();
            _movementCTS?.Dispose();
            _movementCTS = null;
            
            if (newLevelStage == _stageOrder)
            {
                _movementCTS = new CancellationTokenSource();
                var cts = CancellationTokenSource.CreateLinkedTokenSource(_movementCTS.Token, this.GetCancellationTokenOnDestroy());
                MoveCameraAsync(cts.Token).Forget();
            }
        }

        private async UniTask MoveCameraAsync(CancellationToken cancellationToken)
        {
            var distanceVector = (Vector2)(transform.position - _cameraRig.position);
            var speedVector = distanceVector / _transitionDuration;
            var speedLength = math.length(speedVector);

            while (math.length(distanceVector) > speedLength * 0.016f)
            {
                distanceVector = transform.position - _cameraRig.position;
                var shiftVector = speedVector * Time.deltaTime;
                _cameraRig.position += new Vector3(shiftVector.x, shiftVector.y, 0);
                
                await UniTask.NextFrame(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            _cameraRig.position = new Vector3(transform.position.x, transform.position.y, _cameraRig.position.z);
        }
    }
}