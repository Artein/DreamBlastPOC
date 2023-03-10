using TMPro;
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
        [SerializeField, Range(0.1f, 1f), Tooltip("The smaller value, the faster animation is")] 
        private float _smoothAnimationTime;
        
        [SerializeField] private bool _showDownloadProgress;
        [SerializeField] private TMP_Text _downloadProgressText;

        public float SmoothAnimationTime => _smoothAnimationTime;

        private void Awake()
        {
            _downloadProgressText.gameObject.SetActive(_showDownloadProgress);
        }

        public void UpdateDownloadProgressText(long downloadedBytes, long totalBytes)
        {
            _downloadProgressText.text = $"{downloadedBytes} / {totalBytes}";
        }
        
        public void SetBarValue(float progress01)
        {
            progress01 = math.clamp(progress01, 0f, 1f);
            _slider.value = progress01;
        }

        private void OnValidate()
        {
            Assert.IsNotNull(_slider);
            Assert.IsNotNull(_downloadProgressText);
        }
    }
}