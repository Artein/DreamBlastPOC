using System.Globalization;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Utils.Progression
{
    public class Progress : IProgressProvider
    {
        private float _progress01_BF;
        private readonly bool _logProgressChange;
        private readonly string _logMessagePrefix;
        private readonly StringBuilder _logBuilder;
        
        public float Progress01
        {
            get => _progress01_BF;
            set
            {
                var clampedValue = math.clamp(value, 0f, 1f);
                if (Mathf.Approximately(_progress01_BF, clampedValue))
                {
                    return;
                }
                
                var prevValue = _progress01_BF;
                _progress01_BF = clampedValue;

                if (_logProgressChange)
                {
                    _logBuilder.Clear();
                    _logBuilder.Append(_logMessagePrefix);
                    _logBuilder.Append("Progress ");
                    _logBuilder.Append(_progress01_BF.ToString("P", CultureInfo.InvariantCulture));
                    Debug.Log(_logBuilder.ToString());
                }
                
                Changed?.Invoke(_progress01_BF, prevValue);
            }
        }
        
        public event ValueChangeHandler<float> Changed;

        public Progress() : this(logProgressChange: false, logMessagePrefix: null) { }

        public Progress(bool logProgressChange, string logMessagePrefix)
        {
            _logMessagePrefix = logMessagePrefix;
            _logProgressChange = logProgressChange;
            if (_logProgressChange)
            {
                _logBuilder = new StringBuilder();
            }
        }

        public void Reset()
        {
            _progress01_BF = 0f;
        }
    }
}