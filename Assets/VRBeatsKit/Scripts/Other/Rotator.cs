using UnityEngine;

namespace Austral3D
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 axis = Vector3.zero;
        [SerializeField] private float speed = 1.0f;
        [SerializeField] private Space space = Space.Self;

        private void Update()
        {
            transform.Rotate(axis , speed * Time.deltaTime , space);
        }
    }
}

