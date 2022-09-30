using System.Collections.Generic;
using UnityEngine;

namespace NativeSerializableDictionary.Examples
{
    /// <summary>
    /// This example showcases the generic usage of the 
    /// <seealso cref="NativeSerializableDictionary.SerializableDictionary{K, V}"/>
    /// on a GameObject/MonoBehaviour.
    /// </summary>
    public class ExampleMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ExampleScriptableObject example;

        [SerializeField]
        private SerializableDictionary<string, List<DummyGameItem>> gameItemsExample;
        private SerializableDictionary<string, List<DummyGameItem>> GameItemsExample { get => gameItemsExample; }

        // It is safe to modify the values of the dictionary
        // inside of play mode.
        [SerializeField]
        private SerializableDictionary<string, List<Color32>> sample;

        [SerializeField]
        private SerializableDictionary<string, bool> test1;

        [SerializeField]
        private SerializableDictionary<string, List<bool>> test2;


        // This is showcasing a simple data-binding whereby we set the Dictionary
        // to the one we have setup in the ScriptableObject. 
        private void Awake() => gameItemsExample = example.GameItemsExample;
    }
}
