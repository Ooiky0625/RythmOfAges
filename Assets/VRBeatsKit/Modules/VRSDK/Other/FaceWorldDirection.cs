using UnityEngine;

namespace VRSDK
{
    //use by the MuzzleFlashSmall so we can keep the smoke always facing the up direction
    public class FaceWorldDirection : MonoBehaviour
    {
        [SerializeField] private Vector3 direction = Vector3.zero;

        private void LateUpdate()
        {
            transform.forward = direction;
        }
       
    }

}

