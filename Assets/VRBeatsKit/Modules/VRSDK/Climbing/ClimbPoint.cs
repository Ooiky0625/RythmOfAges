using UnityEngine;

namespace VRSDK.Climbing
{
    public class ClimbPoint : VR_Grabbable
    {
        [SerializeField] private ClimbingTarget target = null;
        [SerializeField] private float dropRange = 0.2f;

        private Vector3 startGrabPos = Vector3.zero;
        private Vector3 startLocalPos = Vector3.zero;       
        public bool isActive = false;
        protected override void Start()
        {
            base.Start();

            target = FindObjectOfType<ClimbingTarget>();
            onGrabStateChange.AddListener( OnGrabStateChangeClimb );           
        }

        private void LateUpdate()
        {
            if ( IsActiveOrGrabbed() )
                return;

            if (ShouldDropClimbPoint())
            {
                ForceDrop();
            }            
        }

        private float CalculateDistanceToHand()
        {
            return Vector3.Distance(GetCurrentHandInteractSettings().interactPoint.position, GrabController.OriginalParent.position);
        }

        private bool IsActiveOrGrabbed()
        {
            return isActive || currentGrabState != GrabState.Grab;
        }

        private bool ShouldDropClimbPoint()
        {
            float d = CalculateDistanceToHand();

            return d > dropRange;            
        }

        private void OnGrabStateChangeClimb(GrabState state)
        {
            if (state == GrabState.Grab)
            {                
                transform.SetParent(null);
                Destroy(GrabController.GrabPoint.GetComponent<Joint>());
                GrabController.UseRotationOffset = false;
                GrabController.SetPositionControlMode(MotionControlMode.Free);
                GrabController.transform.SetParent(null);   
                GrabController.transform.position = GetCurrentHandInteractSettings().interactPoint.position;
                GrabController.transform.rotation = Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset ) * GrabController.RotationOffset;
                rb.isKinematic = true;
                target.AddActiveClimbPoint(this);
            }
            else if (state == GrabState.Drop) 
            {
                isActive = false;
            }
                   
        }

        public void SetClimbingPosition()
        {
            if (currentGrabState == GrabState.Grab)
            {
                Vector3 positionDiff = GrabController.OriginalParent.localPosition - startLocalPos;
                positionDiff *= -1;
                Vector3 worldPos = VR_Manager.instance.Player.TrackingSpace.rotation * positionDiff;         
                target.transform.position = (startGrabPos + worldPos);                
            }
        }

        public void OnClimbPointActive()
        {            
            startLocalPos = GrabController.OriginalParent.localPosition;
            startGrabPos = target.transform.position;           
            isActive = true;
        }

    }

}


