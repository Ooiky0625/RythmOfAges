using UnityEngine;

namespace VRBeats
{
    public class BeatCubeDeadZone : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform = null;
        [SerializeField] private Vector3 offset = Vector3.zero;

        private void Awake()
        {
            Collider collider = GetComponent<Collider>();

            if (collider != null)
            {
                collider.isTrigger = true;
            }

        }

        private void LateUpdate()
        {
            transform.position = playerTransform.position + offset;
            
        }

        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log( collider.name );
            TryDestroyCube(collider);
        }

        private void TryDestroyCube(Collider collider)
        {
            VR_BeatCube cube = collider.GetComponent<VR_BeatCube>();

            if (cube != null)
            {
                cube.Kill();
            }            
        }


    }

}
