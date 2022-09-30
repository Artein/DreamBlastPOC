using System;
using System.Collections.Generic;
using UnityEngine;

namespace NativeSerializableDictionary
{
    /// <summary>
    /// This SerializableKVP stores the Key and the Value(s) for the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/>.
    /// </summary>
    /// <typeparam name="K">This is the Key value. It must be unique.</typeparam>
    /// <typeparam name="V">This is the Value associated with the Key.</typeparam>
    /// <remarks>
    /// It is JSON serializable (Tested with <seealso cref="Newtonsoft.Json"/>).
    /// </remarks>
    [Serializable]
    public class SerializableKVP<K, V>
    {
        [SerializeField]
        private K _key;
        public K Key
        {
            get
            {
                return _key;
            }
        }
        [SerializeField]
        private V _value;
        public V Value
        {
            get
            {
                return _value;
            }
        }
    }

    /// <summary>
    /// This SerializableKVP stores the Key and the Value(s) for the <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/>.
    /// </summary>
    /// <typeparam name="K">This is the Key value. It must be unique.</typeparam>
    /// <typeparam name="V">This is the Value associated with the Key.</typeparam>
    /// <remarks>
    /// This alternate version boxes the values into a <seealso cref="System.Collections.Generic.List{T}"/>
    /// so we can bind the data to the List and effectively serialize types like
    /// (see: <seealso cref="UnityEngine.Vector3"/>).
    /// It is JSON serializable (Tested with <seealso cref="Newtonsoft.Json"/>).
    /// </remarks>
    [Serializable]
    public class SerializableKVPBoxed<K, V> : List<V>
    {
        [SerializeField] private K _key;
        public K Key
        {
            get
            {
                return _key;
            }
        }
        [SerializeField] private List<V> _values;
        public List<V> Values
        {
            get
            {
                return _values;
            }
        }
        public V Value
        {
            get
            {
                return _values[0];
            }
        }
    }
}