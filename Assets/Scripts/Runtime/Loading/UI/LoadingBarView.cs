using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.Loading.UI
{
    [DisallowMultipleComponent]
    public class LoadingBarView : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void SetBarValue(float progress01)
        {
            progress01 = math.clamp(progress01, 0f, 1f);
            _slider.value = progress01;
        }

        private void OnValidate()
        {
            Assert.IsNotNull(_slider);
        }
    }
}