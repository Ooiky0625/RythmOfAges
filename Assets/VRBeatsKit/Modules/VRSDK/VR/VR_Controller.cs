using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if SDK_STEAM_VR
using Valve.VR;
#endif


namespace VRSDK
{

    public enum VR_ControllerType
    {
        Left,
        Right
    }

    public enum MotionControlMode
    {
        Free,
        Engine
    }
    

    /// class to asbtrac controller input    
    public class VR_Controller : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private Transform grabPoint = null;
        [SerializeField] private Rigidbody grabPointRB = null;
        [SerializeField] private VR_ControllerType controllerType = VR_ControllerType.Right;
        [SerializeField] protected Animator animator = null;
        [SerializeField] protected AnimationClip defaultInteractAnimationClip = null;
        [SerializeField] private GameObject handGO = null;
        [SerializeField] private VR_DistanceGrab distanceGrab = null;

        #endregion

        #region PUBLIC
        public Transform GrabPoint { get { return grabPoint; } }
        public Vector3 Position { get { return transform.position; } }  

        public Quaternion Rotation { get { return transform.rotation; } }
        public VR_Grabbable CurrentGrab { get { return currentGrab; } private set { currentGrab = value; } }
        public Vector3 AngularVelocity { get { return handPhysics.AngularVelocity; } }
        public Vector3 Velocity { get { return handPhysics.Velocity; } }
        public VR_ControllerType ControllerType { get { return controllerType; } }
        public VR_ControllerGesture GestureScript { get { return gestureScript; } }

        public OnJointBreakListener OnJointBreakListener
        {
            get
            {
                if (onJointBreakListener == null) onJointBreakListener = FindJointBreakListener();
                return onJointBreakListener;
            }
        }
        public Animator Animator { get { return animator; } }
        public virtual Vector3 PositionOffset
        {
            set
            {
                positionOffset = value;

                if (UsePositionOffset && controlPositionMode == MotionControlMode.Engine)
                {
                    transform.localPosition = initialPosition + value;
                }
            }
            get { return positionOffset; }
        }

        public virtual Quaternion RotationOffset
        {
            set
            {
                rotationOffset = value;

                if (UseRotationOffset && controlRotationMode == MotionControlMode.Engine)
                {
                    transform.localRotation = initialRotation * value;
                }

            }
            get { return rotationOffset; }
        }

        public MotionControlMode ControlPositionMode { get { return controlPositionMode; } }
        public MotionControlMode ControlRotationMode { get { return controlRotationMode; } }
        public Transform OriginalParent { get { return originalParent; } }
        public Collider Collider { get { return thisCollider; } }
        public bool UsePositionOffset { get; set; }
        public bool UseRotationOffset { get; set; }
        public Vector3 InitialPosition { get { return initialPosition; } }
        public VR_InputDevice Input { get; private set; }

        public Rigidbody GrabPointRB
        {
            get
            {
                if (grabPointRB == null)
                {
                    var rb = grabPoint.gameObject.GetOrAddComponent<Rigidbody>();
                    grabPointRB = rb;
                }

                return grabPointRB;
            }
        }

        public VR_DistanceGrab DistanceGrab { get => distanceGrab; }
        #endregion

        #region PRIVATE
        private bool initialized = false;
        private HandPhysics handPhysics = null;
        private Vector3 initialPosition = Vector3.zero;
        protected Quaternion initialRotation = Quaternion.identity;
        private List<VR_Interactable> interactList = null;
        private List<VR_Highlight> highlightList = null;
        protected VR_Grabbable currentGrab = null;
        private VR_Highlight currentHighlight = null;
        private OnJointBreakListener onJointBreakListener = null;
        protected AnimatorOverrideController overrideAnimator = null;
        private VR_ControllerGesture gestureScript = null;
        private Transform originalParent = null;
        protected string currentInteractAnimationName = null;
        protected MotionControlMode controlPositionMode = MotionControlMode.Engine;
        protected MotionControlMode controlRotationMode = MotionControlMode.Engine;
        private Collider thisCollider = null;
        protected Vector3 positionOffset = Vector3.zero;
        protected Quaternion rotationOffset = Quaternion.identity;
        private VR_Grabbable activeDistanceGrabbable = null;
        private VR_Highlight activeDistanceHighlight = null;
        private HistoryBuffer historyBuffer = null;
        private VR_Controller otherController
        {
            get
            {
                if (controllerType == VR_ControllerType.Right)
                    return VR_Manager.instance.Player.LeftController;
                else
                    return VR_Manager.instance.Player.RightController;
            }
        }

        #endregion

        private const int historySize = 20;


        #region ANIMATION_HASHES        
        private int isGrabbingHash = -1;
        #endregion


        protected virtual void Awake()
        {
            FindOrCreate_VR_Manager();
            Initialize();
            GetComponents();
            SetupInputDevice();
            
            if (animator != null)
            {
                CreateOverrideAnimator();
                SetupAnimatorHashes();
            }           

        }

        private void FindOrCreate_VR_Manager()
        {
            if (FindObjectOfType<VR_Manager>() == null)
            {
                Debug.LogError("you need a VR_Manager active in the scene in order to use VR Shooter Kit");
            }
        }

        private void SetupInputDevice()
        {
            if (VR_Manager.instance.CurrentSDK == VR_SDK.Oculus)
            {
                Input = new VR_OculusInput(this);
            }
            else if (VR_Manager.instance.CurrentSDK == VR_SDK.Steam_VR)
            {
                Input = new VR_SteamVRInput(this);
            }
            else if (VR_Manager.instance.CurrentSDK == VR_SDK.UnityXR)
            {
                Input = new VR_XRInput(this);
            }
        }

        private void Initialize()
        {
            UsePositionOffset = true;
            UseRotationOffset = true;

            controlPositionMode = MotionControlMode.Engine;
            controlRotationMode = MotionControlMode.Engine;

            SaveLocalPositionAndRotation();
            originalParent = transform.parent;
        }

        private void GetComponents()
        {
            gestureScript = gameObject.GetOrAddComponent<VR_ControllerGesture>();
            thisCollider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            historyBuffer = transform.parent.GetComponent<HistoryBuffer>();
            handPhysics = new HandPhysics( historyBuffer );
        }       


        protected virtual void CreateOverrideAnimator()
        {
            if (animator == null)
                return;
            
            //create override animator controller so we can change the grab animations at running time
            overrideAnimator = new AnimatorOverrideController( animator.runtimeAnimatorController );
            animator.runtimeAnimatorController = overrideAnimator;

            currentInteractAnimationName = defaultInteractAnimationClip.name;
        }

        protected virtual void SetupAnimatorHashes()
        {
            isGrabbingHash = Animator.StringToHash( "IsGrabbing" );
        }

        private void SaveLocalPositionAndRotation()
        {
            //we save this for recentering controllers back later
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

        private OnJointBreakListener FindJointBreakListener()
        {
            OnJointBreakListener listener = grabPoint.GetComponent<OnJointBreakListener>();

            if (listener == null)
                listener = grabPoint.gameObject.AddComponent<OnJointBreakListener>();

            return listener;
        }

        protected virtual void Update()
        {
            UpdateHighlightState();
            if (animator != null) UpdateAnimator();
        }


        private void UpdateHighlightState()
        {
            if (CanHighlight())
            {
                VR_Highlight highlight = FindNearHighlight();

                //if we lost the nearest object
                if (highlight == null && currentHighlight != null)
                {
                    currentHighlight.UnHighlight( this );
                    currentHighlight = null;
                }

                //if we found a new object and we dont have highlight
                if (currentHighlight == null && highlight != null)
                {
                    currentHighlight = highlight;
                    highlight.Highlight( this );
                }

                //if we found a new closer object
                else if (highlight != null && highlight != currentHighlight)
                {
                    currentHighlight.UnHighlight( this );
                    highlight.Highlight( this );
                    currentHighlight = highlight;
                }

                //update the current higlight object, be sure that it is always on
                else if (highlight != null && currentHighlight == highlight && !currentHighlight.IsHighlight)
                {
                    currentHighlight.Highlight( this );
                }
            }
            else if (currentHighlight != null)
            {
                currentHighlight.UnHighlight( this );
                currentHighlight = null;
            }
        }


        protected virtual void UpdateAnimator()
        {
            if (animator.gameObject.activeInHierarchy)
                animator.SetBool( isGrabbingHash, currentGrab != null );
        }

        private bool CanHighlight()
        {
            return currentGrab == null;
        }

        //change the interact animation in running time
        public void OverrideInteractAnimation(AnimationClip animation)
        {
            if (animator == null)
                return;

            if (overrideAnimator == null)
            {
                CreateOverrideAnimator();
            }

            overrideAnimator[currentInteractAnimationName] = animation;
        }

        //back to the default grab animation
        public void SetDefaultInteractAnimation()
        {
            if (animator == null)
                return;

            if (overrideAnimator == null)
            {
                CreateOverrideAnimator();
            }

            overrideAnimator[currentInteractAnimationName] = defaultInteractAnimationClip;
        }

       

        public void ApplyThrowVelocity(VR_Grabbable grabbable)
        {
            handPhysics.ApplyThrowVelocity(grabbable);
        }

                
        public void Recenter()
        {
            PositionOffset = Vector3.zero;
            RotationOffset = Quaternion.identity;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="grabbableList"></param>
        public void Construct(ControllerGestureConfig config)
        {
            if (initialized) return;

            initialized = true;
            //set controller type and get a refenrece to the grabbableList
            this.interactList = VR_Manager.instance.InteractList;
            this.highlightList = VR_Manager.instance.HighlightList;

            VR_ControllerGesture controllerGesture = GetComponent<VR_ControllerGesture>();

            if (controllerGesture != null)
                controllerGesture.Construct(config);
        }


        public void InteractWithNearesObject()
        {
            //we have something grabbe we should no interact
            if (currentGrab != null)
                return;            
            SetDefaultInteractAnimation();

            //get the near interact object to this controller
            VR_Interactable interact = FindNearInteract();
                        

            if (interact == null && activeDistanceGrabbable != null && (activeDistanceGrabbable != otherController.activeDistanceGrabbable || !ThereIsNearbyControllerInteraction(activeDistanceGrabbable)))
            {              
                interact = activeDistanceGrabbable as VR_Interactable;
            }

            if (interact != null && interact.enabled && interact.CanInteractUsingController( this ))
            {
                VR_HandAnimationSettings settings = interact.GetHandAnimationSettings( this );
                ProcessAnimationSettings( settings );
                ProcessInteraction( interact );
            }
        }

        private bool ThereIsNearbyControllerInteraction(VR_Interactable interactable)
        {
            
            if (!otherController.Input.GetButtonDown( interactable.InteractButton ))
            {
                return false;
            }
           
            float thisDistance = CalculateDistanceToInteractable(this , interactable );
            float otherDistance = CalculateDistanceToInteractable( otherController, interactable );

            return thisDistance > otherDistance;
        }

        private float CalculateDistanceToInteractable(VR_Controller controller , VR_Interactable interactable)
        {
            return Vector3.Distance( controller.OriginalParent.position , interactable.transform.position );

        }

        private void ProcessAnimationSettings(VR_HandAnimationSettings settings)
        {            
            if (animator != null && settings != null && settings.animation != null)
            {
                //override the grabbing animation
                OverrideInteractAnimation( settings.animation );
            }

        }

        private void ProcessInteraction(VR_Interactable interact)
        {
            interact.Interact( this );

            if (interact is VR_Grabbable)
            {
                currentGrab = interact as VR_Grabbable;
            }
                
        }
        

        //force a grab no distance check, and drop whathever you have on the hand
        public void ForceGrab(VR_Grabbable grabbable)
        {
            if (grabbable == null)
                return;

            if (currentGrab != null)
                CleanCurrentGrab();

            currentGrab = grabbable;
            currentGrab.OnGrabSuccess( this );
        }       

        public void CleanCurrentGrab()
        {
            currentGrab = null;
        }
       
        public List<Quaternion> GetRotationHistorySample(int sampleCount)
        {
            if (historyBuffer == null)
                return null;

            //return rotationHistory.GetRange( 0, sampleCount > rotationHistory.Count ? rotationHistory.Count : sampleCount);
            return historyBuffer.RotationHistory.Sample(sampleCount);
        }


        /// <summary>
        /// Find the near avalible grabbable to this controller
        /// </summary>
        /// <returns></returns>
        private VR_Interactable FindNearInteract()
        {

            if (interactList.Count == 0)
                return null;

            VR_Interactable interact = null;
            float minDistance = float.MaxValue;

            for (int n = 0; n < interactList.Count; n++)
            {
                if (interactList[n].enabled && interactList[n].CanInteract && interactList[n].CanInteractUsingController( this ))
                {
                    Transform highlightPoint = ( ControllerType == VR_ControllerType.Right ? interactList[n].HighlightPointRightHand : interactList[n].HighlightPointLeftHand );

                    if (highlightPoint != null)
                    {
                        float d = ( Position - highlightPoint.position ).magnitude;
                        
                        if (d < minDistance && CanInteractWithInteractable(interactList[n], d))
                        {
                            interact = interactList[n];
                            minDistance = d;
                        }
                    }

                }

            }


            return interact;
        }

        private bool CanInteractWithInteractable(VR_Interactable interactable, float distance)
        {
            if (interactable == null) return false;
            
            if (interactable.InteractableType == InteractableType.Collider)
            {
                if (interactable.GrabCollider == null)
                    return false;
                
                Vector3 closestPoint = interactable.GrabCollider.ClosestPoint(Position);
                return (Position - closestPoint).magnitude < Mathf.Epsilon;
            }
            else if (interactable.InteractableType == InteractableType.Distance)
            {
                return interactable.InteractDistance >= distance;
            }

            return false;
        }

        /// <summary>
        /// Find the near avalible grabbable to this controller
        /// </summary>
        /// <returns></returns>
        private VR_Highlight FindNearHighlight()
        {

            if (highlightList.Count == 0)
                return null;

            VR_Highlight highlight = null;
            float minDistance = float.MaxValue;

            for (int n = 0; n < highlightList.Count; n++)
            {
                if (highlightList[n].enabled && highlightList[n].CanHighlight() && highlightList[n].CanHighlightUsingController( this ))
                {
                    Transform highlightPoint = ControllerType == VR_ControllerType.Right ? highlightList[n].HighlightPointRightHand : highlightList[n].HighlightPointLeftHand;

                    if (highlightPoint != null)
                    {
                        float d = ( Position - highlightPoint.position ).magnitude;

                        if (d < minDistance && CanInteractWithInteractable(highlightList[n].Interactable, d))
                        {
                            highlight = highlightList[n];
                            minDistance = d;
                        }
                    }

                }

            }

            if (highlight == null)
            {
                highlight = activeDistanceHighlight;
            }
            else if(activeDistanceHighlight != null)
            {
                activeDistanceHighlight.UnHighlight(this);
            }


            return highlight;
        }

        //should the position be controller by the engine or you want to control it manually,
        //useful for snap the hand to certain positions
        public void SetPositionControlMode(MotionControlMode controlMode)
        {
            controlPositionMode = controlMode;

            transform.SetParent( controlMode == MotionControlMode.Free ? null : originalParent);

            if (controlMode == MotionControlMode.Engine)
            {
                transform.localPosition = initialPosition;
                transform.localRotation = initialRotation;
            }

        }

        public void SetRotationControlMode(MotionControlMode controlMode)
        {
            controlRotationMode = controlMode;
        }

        public void SetPositionAndRotationControlMode(MotionControlMode positionControlMode , MotionControlMode rotationControlMode)
        {
            SetPositionControlMode(positionControlMode);
            SetRotationControlMode(rotationControlMode);
        }

        public void SetVisibility(bool visibility)
        {
            if (handGO == null) return;
            handGO.SetActive(visibility);
        }

        public void SetActiveDistanceGrabbable(VR_Grabbable grabbable)
        {
            activeDistanceGrabbable = grabbable;

            if(activeDistanceHighlight != null)
                activeDistanceHighlight.UnHighlight( this );


            if (grabbable != null)
            {
                activeDistanceHighlight = grabbable.GetComponent<VR_Highlight>();
            }
            else
            {
                activeDistanceHighlight = null;
            }
           
        }

    }

}

