using UnityEngine;

namespace Game.Loading.UI
{
    [DisallowMultipleComponent]
    public class LoadingSceneView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public void SetCanvasAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }
    }
}