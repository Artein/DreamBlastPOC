using Unity.Mathematics;
using UnityEngine;

namespace Game.Utils
{
    public class Progress : IProgressProvider
    {
        private float _progress;
        
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
            }
        }
        
        public event IProgressProvider.ChangedHandler Changed;
    }
}