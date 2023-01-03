using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Utils
{
    public class Progress : IProgressProvider
    {
        private float _progress;
        private readonly bool _logProgressChange;
        private readonly string _logMessagePrefix;
        private readonly StringBuilder _logBuilder;
        
        public float Progress01
        {
            get => _progress;
            set
            {
                value = math.clamp(value, 0f, 1f);
                if (!Mathf.Approximately(_progress, value))
                {
                    var prevValue = _progress;
                    _progress = value;
                    Changed?.Invoke(_progress, prevValue);
                }

                if (_logProgressChange)
                {
                    _logBuilder.Clear();
                    _logBuilder.Append(_logMessagePrefix);
                    _logBuilder.Append("Progress ");
                    _logBuilder.Append(_progress * 100f);
                    _logBuilder.Append('%');
                    Debug.Log(_logBuilder.ToString());
                }
            }
        }
        
        public event IProgressProvider.ChangedHandler Changed;

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
    }
}