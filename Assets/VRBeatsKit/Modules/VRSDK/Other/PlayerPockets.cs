using UnityEngine;
using VRSDK.Locomotion;

namespace VRSDK
{
    public class PlayerPockets : MonoBehaviour
    {        
        [SerializeField] private float height = -0.5f;
        [SerializeField] private float lerpSpeed = 0.0f;

        private const float followThreshold = 0.55f;
        private VR_CharacterController characterController = null;

        private Vector3 ThisEulerAngle { get { return transform.rotation.eulerAngles; } }
        private Transform anchorPoint = null;

        private void Start()
        {          
            characterController = FindObjectOfType<VR_CharacterController>();

            if (characterController != null)
            {
                characterController.OnPlayerRotation.AddListener( OnPlayerRotate );
            }

            if (anchorPoint == null)
            {
                Player player = FindObjectOfType<Player>();
                anchorPoint = player.PocketsAnchorPoint;
            }

            SetTeleportCallback();


        }

        private void SetTeleportCallback()
        {
            VR_TeleportHandler teleportHandler = FindObjectOfType<VR_TeleportHandler>();

            if (teleportHandler != null)
            {
                teleportHandler.OnTeleport.AddListener( delegate
                {
                    Quaternion desireRotation = Quaternion.Euler( ThisEulerAngle.x, anchorPoint.rotation.eulerAngles.y, ThisEulerAngle.z );
                    transform.rotation = desireRotation;

                } );

            }
        }

        private void OnPlayerRotate(float angle)
        {           
            Quaternion desireRotation = Quaternion.Euler( ThisEulerAngle.x, ThisEulerAngle.y + angle, ThisEulerAngle.z );
            transform.rotation = desireRotation;
        }

      
        private void LateUpdate()
        {
            SetPosition();
            SetRotationIfWeAreNotLookingAtThePockets();
        }

        private void SetPosition()
        {
            transform.position = anchorPoint.transform.position + ( Vector3.up * height );
        }

        private void SetRotationIfWeAreNotLookingAtThePockets()
        {
            if ( IsLookingAtThePockets() )
            {
                Quaternion desireRotation = CalculateDesireRotation();
                float desireLerpSpeed = CalculateDesireLerpSpeed();

                transform.rotation = Quaternion.Slerp( transform.rotation, desireRotation, desireLerpSpeed );
            }          

        }

        private Quaternion CalculateDesireRotation()
        {
            return Quaternion.Euler( ThisEulerAngle.x, anchorPoint.rotation.eulerAngles.y, ThisEulerAngle.z );
        }

        private float CalculateDesireLerpSpeed()
        {
            Vector3 anchorForward = anchorPoint.forward;
            return Mathf.Abs( lerpSpeed * Mathf.Abs( Mathf.Abs( anchorForward.y ) - 1.0f ) ) * Time.deltaTime;
        }

        private bool IsLookingAtThePockets()
        {
            Vector3 anchorForward = anchorPoint.forward;
            return Mathf.Abs( anchorForward.y ) < followThreshold;
        }

       

    }

}

