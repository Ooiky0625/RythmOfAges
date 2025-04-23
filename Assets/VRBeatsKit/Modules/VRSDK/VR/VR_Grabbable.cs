using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using VRSDK.Events;

namespace VRSDK
{
    public enum GrabState
    {
        UnGrab,
        Flying,
        Grab,
        Drop,
        None
    }

    //this script handles the grabbables
    public class VR_Grabbable : VR_Interactable
    {
        #region INSPECTOR     
        [SerializeField] protected OnGrabStateChangeEvent onGrabStateChange = null;
        [SerializeField] protected bool perfectGrab = false;
        [SerializeField] protected float grabFlyTime = 0.5f;
        [SerializeField] protected bool shouldFly = true;
        [SerializeField] protected bool startOnRightController = false;
        [SerializeField] protected bool startOnLeftController = false;
        [SerializeField] protected bool autoGrab = false;
        [SerializeField] protected bool enableColliderOnGrab = false;
        [SerializeField] protected int grabLayer = 0;
        [SerializeField] protected int unGrabLayer = 0;
        [SerializeField] protected int bulletMaxBounce = 0;        
        [SerializeField] private bool preserveKinematicState = false;
        [SerializeField] private bool toggleGrab = false;      
        [SerializeField] protected List<Collider> ignoreColliderList = new List<Collider>();
        [SerializeField] protected List<Collider> colliderList = null;
        [SerializeField] protected UnityEvent onAfterThrow = null;
        #endregion

        #region protected   
        protected Rigidbody rb = null;
        protected GrabState currentGrabState = GrabState.UnGrab;
        protected VR_Controller activeController = null;
        protected float grabStartTime = 0.0f;
        protected Vector3 grabStartPosition = Vector3.zero;
        protected Quaternion grabStartRotation = Quaternion.identity;
        protected Vector3 childrenPosition = Vector3.zero;       
        protected VR_Tag grabbableTag = null;
        protected Vector3 initialPosition = Vector3.zero;
        protected const float activeDistance = 5.0f;
        protected bool isHighLight = false;       
        protected RigidbodyInterpolation originalInterpolateMode = RigidbodyInterpolation.None;       
        protected bool previousKinematicValue = false;
        protected bool preventDefault = false;
        protected float velocityChangeThreshold = 10f;       
        protected float angularVelocityChangeThreshold = 20f;        
        private bool previousUseGravityState = false;
        private bool previousGravityState = false;              
        private VR_Controller lastInteractController = null;
        private bool objectWasThrow = false;
        protected bool canUseDropZone = true;
        private bool waitAFrameForDrop = false;
        private const float defaultMaxAngularVelocity = 10.0f;
        private float distanceToLeftHand = 0.0f;
        private float distanceToRightHand = 0.0f;
        #endregion

#if UNITY_EDITOR
        [SerializeField] private VR_GrabbableEditorPart editorPart = null;
        public VR_GrabbableEditorPart EditorPart
        {
            get { return editorPart; }
        }
        public bool debug = false;
#endif

        #region PUBLIC    
        public Rigidbody RB { get { return rb; } }
        public List<Collider> ColliderList { get { return colliderList; } }
        public VR_DropZone AffectedDropZone { get; set; }
        public VR_Tag GrabbableTag { get { return grabbableTag; } }
        public float GrabDistance { get { return interactDistance; } }
        public bool IsGrabbed { get { return currentGrabState == GrabState.Grab; } }
        public bool IsHighLight { get { return isHighLight; } }
        public VR_Controller GrabController { get { return activeController; } }
        public OnGrabStateChangeEvent OnGrabStateChange { get { return onGrabStateChange; } }
        public Vector3 PositionOffset { get; set; }
        public Quaternion RotationOffset { get; set; }
        public VR_Controller LastInteractController { get { return lastInteractController; } }      
       
        public bool ObjectWasThrow { get { return objectWasThrow; } }      
        public bool CanUseDropZone { get { return canUseDropZone; } }
        public UnityEvent OnAfterThrow { get { return onAfterThrow; } }
        public GrabState CurrentGrabState
        {
            get
            {
                return currentGrabState;
            }

            private set
            {
                currentGrabState = value;
            }
        }
        public Transform CurrentInteractPoint
        {
            get
            {
                return activeController.ControllerType == VR_ControllerType.Right ? rightHandSettings.interactPoint : leftHandSettings.interactPoint;
            }
        }
        public float GrabFlyTime
        {
            get
            {
                if (shouldFly)
                {
                    return grabFlyTime;
                }

                return 0.0f;
            }
        }    
        
        
        #endregion

        #region UNITY_CALLBACKS
        protected override void Awake()
        {            
            base.Awake();

            Construct();
            SetLayer( unGrabLayer );
            AddRigidBodyIfNeccesary();
            SaveCurrentKinematicState();         
            CalculateDistanceToInteractPoint();
        }

        private void CalculateDistanceToInteractPoint()
        {
            distanceToLeftHand = Vector3.Distance( leftHandSettings.interactPoint.position, transform.position );
            distanceToRightHand = Vector3.Distance( rightHandSettings.interactPoint.position, transform.position );
        }

        public void UpdateGrabPositionOffset()
        {
            CalculateDistanceToInteractPoint();
        }

        private void Construct()
        {           
            grabbableTag = GetComponent<VR_Tag>();
            colliderList = GetComponentsInChildren<Collider>().ToList();
            rb = GetComponent<Rigidbody>();                                       
        }


        protected void SetLayer(int layer)
        {
            gameObject.layer = layer;

            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] != null && !ignoreColliderList.Contains( colliderList[n] ))
                {
                    colliderList[n].gameObject.layer = layer;
                }
                
            }
        }        


        private void AddRigidBodyIfNeccesary()
        {
            if (rb == null)
            {
                Debug.LogWarning( "Grabbable component needs Rigidbody in order to work, adding one" );
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            rb.maxAngularVelocity = defaultMaxAngularVelocity;
        }

        private void SaveCurrentKinematicState()
        {
            if (rb != null)
                previousKinematicValue = rb.isKinematic;
        }

        protected override void Start()
        {
            base.Start();

            //should this grabbable start on the right controller?
            if (startOnRightController)
            {
                VR_Manager.instance.Player.RightController.ForceGrab( this );
            }

            //should this grabbable start on left controller?
            else if (startOnLeftController)
            {               
                VR_Manager.instance.Player.LeftController.ForceGrab( this );
            }
        }       
        
       
        protected override void Update()
        {
            base.Update();

            //call the update
            switch (CurrentGrabState)
            {
                case GrabState.UnGrab:
                UngrabUpdate();
                break;
                case GrabState.Flying:
                FlyUpdate();
                break;
                case GrabState.Grab:
                GrabUpdate();
                break;
                case GrabState.Drop:
                DropUpdate();
                break;
            }

        }

        /// <summary>
        /// in this update the object will be making distance check to the controllers, and check if can be grabbed
        /// </summary>
        protected virtual void UngrabUpdate()
        {
            //wait to be grabbed
            //handle by the VR_Interactable
        }

        /// <summary>
        /// this update is called after a object has been grabbed so it will fly to the hand
        /// </summary>
        protected virtual void FlyUpdate()
        {           
            if ( ShouldFlyToHandPositionAndRotation() )
            {
                MoveToHandPositionAndRotation();
                return;
            }
           
            SetFinalGrabState();

            CurrentGrabState = GrabState.Grab;
            RaiseOnGrabStateChangeEvent( GrabState.Grab );
        }

        private bool ShouldFlyToHandPositionAndRotation()
        {
            float flyPercent = ( Time.time - grabStartTime ) / grabFlyTime;

            return flyPercent < 1 && shouldFly;
        }

        private void MoveToHandPositionAndRotation()
        {
            float flyPercent = ( Time.time - grabStartTime ) / grabFlyTime;

            transform.rotation = Quaternion.Slerp( grabStartRotation , CalculateGrabRotation() , flyPercent );
            transform.position = Vector3.Lerp( grabStartPosition, CalculateGrabPosition(), flyPercent );
        }

        private Quaternion CalculateGrabRotation()
        {           
            Quaternion baseRotation = activeController.GrabPoint.transform.rotation * Quaternion.Euler(GetCurrentHandInteractSettings().CalculateGrabRotationOffset());
            return baseRotation * activeController.Input.GetRotationOffset();
        }

        public VR_HandInteractSettings GetCurrentHandInteractSettings()
        {
            return activeController.ControllerType == VR_ControllerType.Right ? rightHandSettings : leftHandSettings;
        }

        public VR_HandAnimationSettings GetCurrentHandAnimationSettings()
        {
            return activeController.ControllerType == VR_ControllerType.Right ? rightHandAnimationSettings : leftHandAnimationSettings;
        }
             
        private Vector3 CalculateGrabPosition()
        {
            Vector3 dir = ( GetCurrentHandInteractSettings().interactPoint.position - transform.position ).normalized;
            Vector3 grabPosition = activeController.GrabPoint.transform.position + (dir * (CalculateDistanceToCurrentPoint() * -1.0f));

            return grabPosition + activeController.Input.GetPositionOffset();
        }

        private float CalculateDistanceToCurrentPoint()
        {
            if (perfectGrab)
            {
                return Vector3.Distance( GetCurrentHandInteractSettings().interactPoint.position, transform.position );;
            }

            if (activeController.ControllerType == VR_ControllerType.Right) return distanceToRightHand;
            return distanceToLeftHand;
        }

        private void SetFinalGrabState()
        {
            ChangeCollidersEnable( enableColliderOnGrab );
            SetFinalHandPositionAndRotation();
            SetupFixedJoint();
            //should the hand be hide?
            GrabController.SetVisibility( !GetCurrentHandAnimationSettings().hideHandOnGrab );

            //parent the objects so they exist on the same space
            transform.parent = activeController.transform;
        }

        private void ChangeCollidersEnable(bool enable)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if ( CanWeControlThisCollider( colliderList[n] ) )
                {
                    colliderList[n].enabled = enable;
                }
            }
        }

        private bool CanWeControlThisCollider(Collider collider)
        {
            return collider != null && !ignoreColliderList.Contains( collider ) && collider.GetComponent<IgnoreColliderActivationFromGrabbable>() == null;
        }

        private void SetFinalHandPositionAndRotation()
        {
            transform.rotation = CalculateGrabRotation();
            transform.position = CalculateGrabPosition();
        }

        public void SetupFixedJoint()
        {
            DestroyCurrentJoint();
            CreateNewGrabJoint();            

            activeController.OnJointBreakListener.SetListener( OnJointBreak );
            rb.isKinematic = false;
        }

        private void DestroyCurrentJoint()
        {
            FixedJoint joint = activeController.GrabPoint.gameObject.GetComponent<FixedJoint>();

            if (joint != null)
                Destroy( joint );
        }

        private void CreateNewGrabJoint()
        {
            var grabRB = activeController.GrabPoint.gameObject.GetComponent<Rigidbody>();

            if (grabRB == null)
            {
                grabRB = activeController.GrabPoint.gameObject.AddComponent<Rigidbody>();
            }

            grabRB.isKinematic = true;
            grabRB.useGravity = false;
            
            FixedJoint joint = activeController.GrabPoint.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rb;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;
        }

        /// <summary>
        /// update when a this object is grabbed
        /// </summary>
        protected virtual void GrabUpdate()
        {
            //check if we should drop this grabbable
            if (ShouldDropObject())
            {
                CurrentGrabState = GrabState.Drop;
                return;
            }

            UpdatePositionAndRotationOffset();

        }

        private bool ShouldDropObject()
        {
            if (waitAFrameForDrop)
            {
                waitAFrameForDrop = false;
                return false;
            }

            if (toggleGrab)
            {
                return activeController.Input.GetButtonDown( interactButton );
            }

            return !autoGrab && !activeController.Input.GetButton( interactButton );
        }

        protected void UpdatePositionAndRotationOffset()
        {
            activeController.PositionOffset = PositionOffset;
            activeController.RotationOffset = RotationOffset;
        }

        /// <summary>
        /// called when the object is dropped, this is just called a frame mainly it is a excuse to call RaiseOnGrabStateChangeEvent
        /// </summary>
        protected virtual void DropUpdate()
        {
            if (activeController == null)
            {
                //can be grabbed aigan
                CanInteract = true;   
                TransitionToUnGrabState();  
                return;
            }
            
            ResetActiveControllerState();

            //some componets stop the default behaivour of this component like the VR_TwoHandGrabbable.cs
            if (!preventDefault)
            {
                ResetRigidBodyState();
                ApplyControllerVelocity();
                EnableHandCollision();
                ChangeCollidersEnable( true );
                transform.SetParent( null );
            }

            lastInteractController = activeController;
            activeController = null;

            //can be grabbed aigan
            CanInteract = true;   
            TransitionToUnGrabState();            
        }

        private void ResetActiveControllerState()
        {
            activeController.UsePositionOffset = true;
            activeController.UseRotationOffset = true;

            activeController.SetPositionAndRotationControlMode( MotionControlMode.Engine, MotionControlMode.Engine );

            GrabController.SetVisibility( true );

            if (activeController != null)
            {
                ResetControllerState( activeController );
            }
        }

        private void ResetRigidBodyState()
        {
            if (rb != null)
                rb.interpolation = originalInterpolateMode;

            if (preserveKinematicState && rb != null)
            {
                rb.isKinematic = previousKinematicValue;
            }
            else if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        private void EnableHandCollision()
        {
            if (activeController.Velocity.magnitude > 0.1f && activeController.Collider != null)
            {
                StartCoroutine( EnableCollisionRoutine( activeController.Collider, 0.1f ) );
            }
            else if (activeController.Collider != null)
            {
                EnableCollision( activeController.Collider );
            }
        }


        private void TransitionToUnGrabState()
        {
            RaiseOnGrabStateChangeEvent( GrabState.Drop );
            CurrentGrabState = GrabState.UnGrab;
            RaiseOnGrabStateChangeEvent( GrabState.UnGrab );
        }
              

        private void SetRigidbodyVelocityToZero()
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        
#endregion

        //this is a feature what i am working you can ignore this function :)
        public void SetEditorGrabPositionAndRotation(VR_Controller controller)
        {
            activeController = controller;
            
            if (shareHandInteractionSettings)
            {
                leftHandSettings = handSettings;
                rightHandSettings = handSettings;
            }

            transform.position = CalculateGrabPosition();
            transform.rotation = activeController.GrabPoint.transform.rotation * Quaternion.Euler( GetCurrentHandInteractSettings().rotationOffset );
                     
            //SetupFixedJoint();
            transform.parent = activeController.transform;
        }

        //this is a feature what i am working you can ignore this function :)
        public void CopySettingsTo(VR_Grabbable grabbable)
        {
            grabbable.handSettings.rotationOffset = handSettings.rotationOffset;
            grabbable.rightHandSettings.rotationOffset = rightHandSettings.rotationOffset;
            grabbable.leftHandSettings.rotationOffset = leftHandSettings.rotationOffset;
        }
        
        private void ApplyControllerVelocity()
        {
            if (rb != null && activeController != null)
            {
                StartCoroutine(ApplyControllerVelocityRoutine(activeController));                
            }
        }

        private IEnumerator ApplyControllerVelocityRoutine(VR_Controller controller)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            yield return new WaitForFixedUpdate();
           
            controller.ApplyThrowVelocity(this);
            

            while (rb.velocity.magnitude < 0.25f)
                yield return new WaitForFixedUpdate();

            onAfterThrow.Invoke();
            objectWasThrow = true;
        }

        private void ResetControllerState(VR_Controller controller)
        {
            //recenter controller
            controller.Recenter();

            controller.OnJointBreakListener.RemoveAllListeners();
            FixedJoint joint = controller.GrabPoint.gameObject.GetComponent<FixedJoint>();

            if (joint != null)
                Destroy( joint );

            controller.CleanCurrentGrab();
        }
       
        protected void RaiseOnGrabStateChangeEvent(GrabState grabState)
        {
            SetLayer( grabState == GrabState.Grab ? grabLayer : unGrabLayer );
            onGrabStateChange.Invoke( grabState );
        }       

        public void ForceDrop()
        {
            m_buttonWasPressedLeft = VR_Manager.instance.Player.LeftController.Input.GetButtonDown( interactButton );
            m_buttonWasPressedRight = VR_Manager.instance.Player.RightController.Input.GetButtonDown( interactButton );

            DropUpdate();
        }

        public override void Interact(VR_Controller controller)
        {
            OnGrabSuccess( controller );
        }

        /// <summary>
        /// Called by VR_Input, to let know what we are grabbing this object
        /// </summary>
        /// <param name="controller"></param>
        public virtual void OnGrabSuccess(VR_Controller controller)
        {
            if (preventDefault)
            {
                waitAFrameForDrop = true;
                activeController = controller;
                CurrentGrabState = GrabState.Grab;                
                RaiseOnGrabStateChangeEvent(CurrentGrabState);
                return;
            }

            previousKinematicValue = rb.isKinematic;

            //stop this object to be interactable
            CanInteract = false;

            //set the active controller
            activeController = controller;

           
            if (rb != null)
            {
                rb.isKinematic = true;
                originalInterpolateMode = rb.interpolation;
                rb.interpolation = RigidbodyInterpolation.None;                
            }
                        

            //disable collision with the grabbable and the hand
            if (activeController.Collider != null)
            {
                IgnoreCollision(activeController.Collider);
            }           


            //if this object shoudl fly to hand disable colliders while flying otherwise set desire collider state
            if (shouldFly)
            {
                //disable colliders while flying
                ChangeCollidersEnable( false );
            }
            else
            {
                waitAFrameForDrop = true;
                ChangeCollidersEnable( enableColliderOnGrab );
            }

            //if we are using a perfect grab
            if (perfectGrab)
            {
                //parent the objects so they exist on the same space
                transform.parent = activeController.transform;
                GrabController.SetVisibility( !GetCurrentHandAnimationSettings().hideHandOnGrab );
                SetupFixedJoint();               

                //set the current grab state
                CurrentGrabState = GrabState.Grab;
                //raise grab state change event
                RaiseOnGrabStateChangeEvent( CurrentGrabState );
                return;
            }
            else
            {

                //set fly values
                if (shouldFly)
                {
                    grabStartTime = Time.time;
                    grabStartPosition = transform.position;
                    grabStartRotation = transform.rotation;
                }

                else
                {
                    SetFinalGrabState();
                }

                CurrentGrabState = shouldFly ? GrabState.Flying : GrabState.Grab;

            }


            //raise the event
            RaiseOnGrabStateChangeEvent( ( shouldFly ? GrabState.Flying : GrabState.Grab ) );

        }

        public void PreventDefault()
        {
            preventDefault = true;
        }
                      

        /// <summary>
        /// Called whena grabbed joints breaks
        /// </summary>
        private void OnJointBreak(float f)
        {
            //if the object is no grabbed ignore jojntbreak
            if (CurrentGrabState != GrabState.Grab)
                return;

            FixedJoint joint = activeController.GrabPoint.gameObject.GetComponent<FixedJoint>();

            if (joint != null && joint.connectedBody != null)
                return;

            m_buttonWasPressedLeft = VR_Manager.instance.Player.LeftController.Input.GetButtonDown( interactButton );
            m_buttonWasPressedRight = VR_Manager.instance.Player.RightController.Input.GetButtonDown( interactButton );


            CurrentGrabState = GrabState.Drop;
        }

        private void IgnoreCollision(Collider c)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] != null && c != null)
                {
                    Physics.IgnoreCollision( colliderList[n], c );
                }
                
            }
        }

        private IEnumerator EnableCollisionRoutine(Collider c, float t)
        {
            yield return new WaitForSeconds( t );
            IgnoreCollision( c );
        }

        private void EnableCollision(Collider c)
        {
            for (int n = 0; n < colliderList.Count; n++)
            {
                if (colliderList[n] != null)
                {
                    Physics.IgnoreCollision( colliderList[n], c, false );
                }
               
            }
        }


        public void OnDropSuccess()
        {
            ChangeCollidersEnable( false );
        }        

        public void IgnoreCollider(Collider c)
        {
            ignoreColliderList.Add( c );
        }

        public void SetStartOnLeftHand(bool value)
        {
            startOnLeftController = value;
        }

        public void SetStartOnRightHand(bool value)
        {
            startOnRightController = value;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onGrabStateChange.RemoveAllListeners();
        }
    }
}