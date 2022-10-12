using System;
using UnityEngine;

namespace Game.Helpers
{
    [DisallowMultipleComponent]
    public class DestroyNotifier : MonoBehaviour
    {
        public event Action<DestroyNotifier> Destroying;
        
        private void OnDestroy()
        {
            Destroying?.Invoke(this);
        }
    }
}