using UnityEngine;

namespace VRSDK
{
    public class PreserveRotation : MonoBehaviour
    {
        private Vector3 forward = Vector3.zero;

        private void Awake()
        {
            forward = transform.forward;
        }

        private void LateUpdate()
        {
            transform.forward = forward;
        }
    }
}

