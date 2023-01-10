using System.Threading;
using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Loading.UI
{
    [DisallowMultipleComponent]
    public class LoadingSceneView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private MMF_Player _disappearAnimationPlayer;

        public async UniTask StartPlayingDisappearAnimationAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _disappearAnimationPlayer.PlayFeedbacks();

            while (_disappearAnimationPlayer.IsPlaying)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            }
        }

        public void SetCanvasAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }
        
        [Button("TEST PLAY")]
        private void TestPlay()
        {
            _disappearAnimationPlayer.PlayFeedbacks();
        }
    }
}