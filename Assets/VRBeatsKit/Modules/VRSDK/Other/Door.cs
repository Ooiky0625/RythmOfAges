using UnityEngine;

namespace VRSDK
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private float throwForce = 100.0f;

        private VR_Grabbable grabbable = null;
        private VR_Controller grabController = null;

        private void Awake()
        {
            grabbable = GetComponent<VR_Grabbable>();
            grabbable.OnGrabStateChange.AddListener(OnGrabStateChange);
        }

        private void OnGrabStateChange(GrabState newState)
        {
            if (newState == GrabState.Grab)
            {
                grabController = grabbable.GrabController;
            }

            if (newState == GrabState.Drop)
            {
                Debug.Log("drop");
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().AddForceAtPosition( grabController.Velocity * throwForce, grabController.transform.position );
            }
        }
        
    }

}

