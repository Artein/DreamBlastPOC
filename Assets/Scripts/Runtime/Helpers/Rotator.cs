using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Helpers
{
    [DisallowMultipleComponent]
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private bool3 _initialRandomRotation;
        [SerializeField] private Vector3 _eulerAnglesPerSecond;

        private void Awake()
        {
            if (_initialRandomRotation.x)
            {
                transform.Rotate(Vector3.right, Random.Range(0, 360));
            }

            if (_initialRandomRotation.y)
            {
                transform.Rotate(Vector3.up, Random.Range(0, 360));
            }
            
            if (_initialRandomRotation.z)
            {
                transform.Rotate(Vector3.forward, Random.Range(0, 360));
            }
        }

        private void Update()
        {
            var rotationAngle = _eulerAnglesPerSecond * Time.deltaTime;
            transform.Rotate(rotationAngle);
        }
    }
}