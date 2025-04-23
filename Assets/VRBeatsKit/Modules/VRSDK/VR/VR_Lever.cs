using UnityEngine;
using VRSDK.Events;

namespace VRSDK
{
    //this script controls the lever, the lever is completely based on physics so it can respond to any collision
    public class VR_Lever : VR_Grabbable
    {
        [SerializeField] private Transform transformBase = null;
        [SerializeField] private int solverIterations = 10;
        [SerializeField] private OnValueChangeEvent onValueChange = null;
        [SerializeField] private bool shouldBackToStartingPosition = false;
        [SerializeField] private float backForce = 2.0f;

        private HingeJoint joint = null;
        private float initialAngle = 0.0f;
        private float lastValue = -1.0f;
        private Vector3 initialDir = Vector3.zero;
        private float movementRange = 0.0f;
        private Collider thisCollider = null;
        private Collider triggerHandCollider = null;
        private bool isIgnoringHandCollision = false;

        private const float MAX_FORCE = 1000.0f;

        public OnValueChangeEvent OnValueChange { get { return onValueChange; } }

        protected override void Awake()
        {            
            base.Awake();

            canUseDropZone = false;
            thisCollider = GetComponent<Collider>();
            joint = GetComponent<HingeJoint>();
            rb = GetComponent<Rigidbody>();

            //solver iterations help to get a better physics behaviour
            //but be careful this can slowdown the game
            rb.solverIterations = solverIterations;

            //we dont need this
            shouldFly = false;
            autoGrab = false;

            //ignore base collision
            Collider[] colliderArray = transformBase.GetComponentsInChildren<Collider>();

            for (int n = 0; n < colliderArray.Length; n++)
            {
                Physics.IgnoreCollision( thisCollider, colliderArray[n] );
            }

            //initialize the joint
            SetUpHingeJoint();

            OnGrabStateChange.AddListener( OnGrabStateChangeCallback );

        }

        private void SetUpHingeJoint()
        {
            initialAngle = GetCurrentAngle();
            movementRange = Mathf.Abs( joint.limits.min - joint.limits.max );

            initialDir = transform.InverseTransformDirection( Vector3.up );

            joint.useMotor = false;
            joint.useSpring = shouldBackToStartingPosition;

            //cancel all forces          
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;


            //set back force
            JointSpring spring = joint.spring;
            spring.spring = backForce;
            spring.targetPosition = 0.0f;
            joint.spring = spring;

        }


        protected override void Update()
        {
            base.Update();

            float newValue = Mathf.Clamp( GetClampAngle() / movementRange, 0.0f, 1.0f );


            if (newValue != lastValue)
            {
                onValueChange.Invoke( newValue );
                lastValue = newValue;
            }

            if (isIgnoringHandCollision && Vector3.Distance( triggerHandCollider.transform.position, RightInteractPoint.position ) > interactDistance)
            {
                Physics.IgnoreCollision( thisCollider, triggerHandCollider, false );
                isIgnoringHandCollision = false;
            }

        }

        private void OnGrabStateChangeCallback(GrabState state)
        {
            if (state == GrabState.Grab)
            {
                if (thisCollider != null)
                {
                    thisCollider.enabled = false;
                }

                triggerHandCollider = activeController.GetComponent<Collider>();

                JointSpring spring = joint.spring;
                spring.spring = MAX_FORCE;
                joint.spring = spring;


                joint.useMotor = false;
                joint.useSpring = true;
            }

            else if (state == GrabState.UnGrab)
            {
                if (triggerHandCollider != null)
                {
                    Physics.IgnoreCollision( thisCollider, triggerHandCollider, true );
                    isIgnoringHandCollision = true;
                }
                else
                {
                    isIgnoringHandCollision = false;
                }

                if (thisCollider != null)
                {
                    thisCollider.enabled = true;
                }

                if (shouldBackToStartingPosition)
                {
                    SetupBackLever();
                }
            }
        }


        private void SetupBackLever()
        {
            JointSpring spring = joint.spring;
            spring.spring = backForce;
            spring.targetPosition = 0.0f;
            joint.spring = spring;


            joint.useMotor = false;
            joint.useSpring = true;
        }


        private float GetCurrentAngle()
        {

            if (joint.axis.x > 0.0f)
                return WrapAngle( transform.rotation.eulerAngles.x );
            else if (joint.axis.y > 0.0f)
                return WrapAngle( transform.rotation.eulerAngles.y );
            else if (joint.axis.z > 0.0f)
                return WrapAngle( transform.rotation.eulerAngles.z );

            return 0.0f;
        }

        //convert angle to human readable
        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }



        protected override void GrabUpdate()
        {
            base.GrabUpdate();

            UpdateJoint();

            Transform highlightPoint = activeController.ControllerType == VR_ControllerType.Right ? HighlightPointRightHand : HighlightPointLeftHand;

            float distance = ( highlightPoint.transform.position - activeController.GrabPoint.transform.position ).magnitude;

            if (distance > interactDistance)
            {
                currentGrabState = GrabState.Drop;
                RaiseOnGrabStateChangeEvent( GrabState.Drop );
            }

        }

        protected override void DropUpdate()
        {
            GrabController.SetVisibility( true );

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            joint.useSpring = false;

            activeController.CleanCurrentGrab();
            activeController = null;

            RaiseOnGrabStateChangeEvent( GrabState.Drop );
            currentGrabState = GrabState.UnGrab;
            RaiseOnGrabStateChangeEvent( GrabState.UnGrab );

        }

        public override void OnGrabSuccess(VR_Controller controller)
        {
            activeController = controller;
            joint.useSpring = true;

            currentGrabState = GrabState.Grab;
            RaiseOnGrabStateChangeEvent( GrabState.Grab );

            GrabController.SetVisibility( !GetCurrentHandAnimationSettings().hideHandOnGrab );

        }

        private float GetAngle()
        {
            if (joint.axis.x > 0.0f)
            {
                Vector3 dir = transformBase.transform.position - activeController.GrabPoint.transform.position;
                Vector3 myDir = transformBase.up * -1.0f;

                return Vector2.SignedAngle( new Vector2( dir.z, dir.y ), new Vector2( 0.0f, myDir.y ) );
            }

            else if (joint.axis.z > 0.0f)
            {
                Vector3 dir = transformBase.transform.position - activeController.GrabPoint.transform.position;
                Vector3 myDir = transformBase.up * -1.0f;

                return Vector2.SignedAngle( new Vector2( dir.x * -1.0f, dir.y ), new Vector2( myDir.x, myDir.y ) );
            }

            else if (joint.axis.y > 0.0f)
            {
                Vector3 dir = transformBase.transform.position - activeController.GrabPoint.transform.position;
                Vector3 myDir = transformBase.right *-1.0f;

                return Vector2.SignedAngle( new Vector2( dir.x, dir.z ), new Vector2( myDir.x, myDir.z ) );
            }

            return 0.0f;


        }

        private float GetClampAngle()
        {

            if (joint.axis.z > 0.0f)
                return Vector2.Angle( new Vector2( transform.up.x, transform.up.y ), new Vector2( initialDir.x, initialDir.y ) );
            if (joint.axis.x > 0.0f)
                return Vector2.Angle( new Vector2( transform.up.z, transform.up.y ), new Vector2( initialDir.z, initialDir.y ) );

            return 0.0f;
        }


        private void UpdateJoint()
        {

            float visualAngle = GetAngle() - initialAngle;


            JointSpring spring = joint.spring;
            spring.targetPosition = visualAngle;

            joint.spring = spring;

        }


    }

}

