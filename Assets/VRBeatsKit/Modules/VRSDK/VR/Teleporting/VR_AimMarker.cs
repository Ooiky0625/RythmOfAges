using UnityEngine;

namespace VRSDK.Locomotion
{
    //this scripts handles aim marker position and rotation
    public class VR_AimMarker : MonoBehaviour
    {
        [SerializeField] private GameObject marker = null;
        [SerializeField] private BoxCollider collider = null;
        [SerializeField] private float slopeLimit;

        public GameObject Marker { get { return marker; } }
        public BoxCollider Collider { get { return collider; } }
        public float SlopeLimit { get { return slopeLimit; } }

        private void Awake()
        {
            //disable marker 
            marker.gameObject.SetActive(false);
        }

        public void Hide()
        {
            marker.SetActive( false );
        }

        public void UpdatePositionAndRotation(VR_Controller controller, AimRaycastInfo info, bool active = true)
        {
            if (!marker.activeInHierarchy && active)
                marker.SetActive( true );

            marker.transform.position = info.hitPoint;
            marker.transform.up = info.normal;


            Vector2 controllerInput = controller.Input.GetJoystick().normalized;
            Vector3 controllerDirection = new Vector3( controllerInput.x, 0.0f, controllerInput.y );

            //get controller pointing direction in world space
            controllerDirection = controller.transform.TransformDirection( controllerDirection );                   
           
            //get marker forward in local space
            Vector3 forward = marker.transform.InverseTransformDirection( marker.transform.forward);
           
            //find the angle diference betwen the controller pointing direction and marker current forward
            float angle = Vector2.SignedAngle( new Vector2( controllerDirection.x , controllerDirection.z ) , new Vector2( forward.x , forward.z ) );

            //rotate marker in local space to match controller pointing direction
            marker.transform.Rotate(Vector3.up , angle , Space.Self);          

        }
        
    }
}

