using UnityEngine;

namespace Game.Helpers
{
    public class InitialRotationLocker : MonoBehaviour
    {
        private Vector3 _initialWorldOffset;
        private Quaternion _initialRotation;
        
        private void Awake()
        {
            _initialRotation = transform.rotation;
            _initialWorldOffset = transform.position - transform.parent.position;
        }

        private void Update()
        {
            var tr = transform;
            tr.rotation = _initialRotation;
            tr.position = tr.parent.position + _initialWorldOffset;
        }
    }
}