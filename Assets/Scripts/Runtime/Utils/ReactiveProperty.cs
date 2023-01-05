using System;
using System.Text;
using JetBrains.Annotations;

namespace Game.Utils
{
    // TODO: Make 2 separate versions ReactiveProperty<T> and ReactivePropertyU<T> (for unmanaged types to eliminate GC)
    // TODO: Make a version with subscription/get/read-only access to eliminate changes outside of the containing object
    public class ReactiveProperty<T>
    {
        private T _value;
        private bool _initialized;
        private readonly bool _logEveryValueChange;
        private readonly StringBuilder _stringBuilder;
        private readonly UnityEngine.Object _logContext = null;
        private readonly string _logPrefix = string.Empty;
        private event ValueChangeHandler<T> ValueChangedInternal;
        
        public T Value
        {
            get => _value;
            set
            {
                _initialized = true;
                if (Equals(_value, value))
                {
                    return;
                }

                var prevValue = _value;
                _value = value;
                if (_logEveryValueChange)
                {
                    _stringBuilder.Append(_logPrefix);
                    _stringBuilder.Append(" value changed to '");
                    _stringBuilder.Append(value);
                    _stringBuilder.Append('\'');
                    var logMessage = _stringBuilder.ToString();
                    if (_logContext != null)
                    {
                        UnityEngine.Debug.Log(logMessage, _logContext);
                    }
                    else
                    {
                        UnityEngine.Debug.Log(logMessage);
                    }

                    _stringBuilder.Clear();
                }
                ValueChangedInternal?.Invoke(_value, prevValue);
            }
        }

        // During subscription you will immediately receive event with current value (if only initialized)
        public event ValueChangeHandler<T> ValueChanged
        {
            add
            {
                ValueChangedInternal += value;
                if (_initialized)
                {
                    value?.Invoke(_value, default);
                }
            }
            remove => ValueChangedInternal -= value;
        }
        
        // During subscription you will not receive event with current (initialize) value
        public event ValueChangeHandler<T> ValueChanged_SkipInitializeEvent
        {
            add => ValueChangedInternal += value;
            remove => ValueChangedInternal -= value;
        }
        
        public ReactiveProperty()
        {
            _value = default;
        }

        public ReactiveProperty(T initialValue)
        {
            _value = initialValue;
            _initialized = true;
        }

        public ReactiveProperty(T initialValue, [NotNull] string everyChangeLogPrefix, [CanBeNull] UnityEngine.Object context) : this(initialValue)
        {
            if (string.IsNullOrEmpty(everyChangeLogPrefix))
            {
                throw new ArgumentException("Wrong argument", nameof(everyChangeLogPrefix));
            }
            
            _logEveryValueChange = true;
            _stringBuilder = new StringBuilder();
            _logPrefix = everyChangeLogPrefix;
        }

        public static implicit operator T(ReactiveProperty<T> property)
        {
            return property.Value;
        }

        public static implicit operator ReactiveProperty<T>(T value)
        {
            return new ReactiveProperty<T>(value);
        }
    }
}