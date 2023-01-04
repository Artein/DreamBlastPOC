using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.UI
{
    [DisallowMultipleComponent]
    public class SliderValueLabel : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _textBegin;
        [SerializeField] private string _textEnd;
        
        [SerializeField, Tooltip("https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings")] 
        private string _sliderValueToStringFormat;
        
        private readonly StringBuilder _stringBuilder = new();

        private void Awake()
        {
            OnSliderValueChanged(_slider.value);
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(_textBegin);
            _stringBuilder.Append(value.ToString(_sliderValueToStringFormat, CultureInfo.InvariantCulture));
            _stringBuilder.Append(_textEnd);
            _text.text = _stringBuilder.ToString();
        }

        private void OnValidate()
        {
            Assert.IsNotNull(_slider);
            Assert.IsNotNull(_text);
        }
    }
}