using UnityEngine;
using System;
using VRSDK.Events;

namespace VRSDK
{

    [RequireComponent(typeof(CharacterController))]
    public class VR_CharacterController : MonoBehaviour, ISpeed
    {
        public Transform forwardTransform = null;

        /// <summary>
        /// The rate acceleration during movement.
        /// </summary>
        public float Acceleration = 0.1f;

        public float AirSpeedControl = 0.5f;

        /// <summary>
        /// The rate of damping on movement.
        /// </summary>
        public float Damping = 0.3f;

        /// <summary>
        /// The rate of additional damping when moving sideways or backwards.
        /// </summary>
        public float BackAndSideDampen = 0.5f;

        /// <summary>
        /// The force applied to the character when jumping.
        /// </summary>
        public float JumpForce = 0.3f;

        /// <summary>
        /// The rate of rotation when using a gamepad.
        /// </summary>
        public float RotationAmount = 1.5f;

        /// <summary>
        /// The rate of rotation when using the keyboard.
        /// </summary>
        public float RotationRatchet = 45.0f;

        /// <summary>
        /// The player will rotate in fixed steps if Snap Rotation is enabled.
        /// </summary>
        [Tooltip("The player will rotate in fixed steps if Snap Rotation is enabled.")]
        public bool SnapRotation = true;

        /// <summary>
        /// How many fixed speeds to use with linear movement? 0=linear control
        /// </summary>
        [Tooltip("How many fixed speeds to use with linear movement? 0=linear control")]
        public int FixedSpeedSteps;


#if SDK_OCOLUS
         /// <summary>
        /// If true, reset the initial yaw of the player controller when the Hmd pose is recentered.
        /// </summary>
        public bool HmdResetsY = true;

        /// <summary>
        /// If true, tracking data from a child OVRCameraRig will update the direction of movement.
        /// </summary>
        public bool HmdRotatesY = true;
#endif



        /// <summary>
        /// Modifies the strength of gravity.
        /// </summary>
        public float GravityModifier = 0.379f;

#if SDK_OCULUS
        /// <summary>
        /// If true, each OVRPlayerController will use the player's physical height.
        /// </summary>
        public bool useProfileData = true;
#endif

        /// <summary>
        /// The CameraHeight is the actual height of the HMD and can be used to adjust the height of the character controller, which will affect the
        /// ability of the character to move into areas with a low ceiling.
        /// </summary>
        [NonSerialized]
        public float CameraHeight;

        /// <summary>
        /// This event is raised after the character controller is moved. This is used by the OVRAvatarLocomotion script to keep the avatar transform synchronized
        /// with the OVRPlayerController.
        /// </summary>
        public event Action<Transform> TransformUpdated;

        /// <summary>
        /// This bool is set to true whenever the player controller has been teleported. It is reset after every frame. Some systems, such as
        /// CharacterCameraConstraint, test this boolean in order to disable logic that moves the character controller immediately
        /// following the teleport.
        /// </summary>
        [NonSerialized] // This doesn't need to be visible in the inspector.
        public bool Teleported;

        /// <summary>
        /// This event is raised immediately after the camera transform has been updated, but before movement is updated.
        /// </summary>
        public event Action CameraUpdated;

        /// <summary>
        /// This event is raised right before the character controller is actually moved in order to provide other systems the opportunity to
        /// move the character controller in response to things other than user input, such as movement of the HMD. See CharacterCameraConstraint.cs
        /// for an example of this.
        /// </summary>
        public event Action PreCharacterMove;

        /// <summary>
        /// When true, user input will be applied to linear movement. Set this to false whenever the player controller needs to ignore input for
        /// linear movement.
        /// </summary>
        public bool EnableLinearMovement = true;

        /// <summary>
        /// When true, user input will be applied to rotation. Set this to false whenever the player controller needs to ignore input for rotation.
        /// </summary>
        public bool EnableRotation { get; set; }

        public Vector3 Velocity { get { return velocity; }
            set
            {
                velocity = value;
            }
        }

#if SDK_OCULUS
        public bool HmdRotatesY = true;
        public bool HmdResetsY = true;

#endif

        [SerializeField] private OnValueChangeEvent onPlayerRotation = null;



        protected CharacterController Controller = null;
#if SDK_OCULUS
        protected OVRCameraRig CameraRig = null;
        private OVRPose? InitialPose;
        private bool prevHatLeft = false;
        private bool prevHatRight = false;
        private bool HaltUpdateMovement = false;

#endif
        
        private VR_Controller rightController = null;
        private VR_Controller leftController = null;
        private float MoveScale = 1.0f;
        private Vector3 MoveThrottle = Vector3.zero;
        private float FallSpeed = 0.0f;

        public bool StopSimulation = false;
        public float InitialYRotation { get; private set; }
        public OnValueChangeEvent OnPlayerRotation { get { return onPlayerRotation; } }

        public float Speed
        {
            get
            {
                return speed;
                //return Acceleration * leftController.Input.GetJoystickInput().sqrMagnitude * Time.deltaTime;
            }
        }

        private float MoveScaleMultiplier = 1.0f;
        private float RotationScaleMultiplier = 1.0f;
        private bool SkipMouseRotation = true; // It is rare to want to use mouse movement in VR, so ignore the mouse by default.                
        private float SimulationRate = 60f;
        private float buttonRotation = 0f;
        private bool ReadyToSnapTurn; // Set to true when a snap turn has occurred, code requires one frame of centered thumbstick to enable another snap turn.
        private bool playerControllerEnabled = false;
        private float moveInfluence = 0.0f;
        public Vector3 lastPosition = Vector3.zero;
        private Vector3 velocity = Vector3.zero;
        private float originalGravity = 0.0f;



        void Start()
        {
#if SDK_OCULUS
            // Add eye-depth as a camera offset from the player controller
            var p = CameraRig.transform.localPosition;
            p.z = OVRManager.profile.eyeDepth;
            CameraRig.transform.localPosition = p;
#endif

            originalGravity = GravityModifier;
        }

        void Awake()
        {
            EnableRotation = true;
            Controller = gameObject.GetComponent<CharacterController>();

            if (Controller == null)
                Debug.LogWarning("VR_CharacterController: No CharacterController attached.");

#if SDK_OCULUS
            // We use OVRCameraRig to set rotations to cameras,
            // and to be influenced by rotation
            OVRCameraRig[] CameraRigs = gameObject.GetComponentsInChildren<OVRCameraRig>();

            if (CameraRigs.Length == 0)
                Debug.LogWarning("OVRPlayerController: No OVRCameraRig attached.");
            else if (CameraRigs.Length > 1)
                Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraRig attached.");
            else
                CameraRig = CameraRigs[0];
#endif

            InitialYRotation = transform.rotation.eulerAngles.y;

            rightController = VR_Manager.instance.Player.RightController;
            leftController = VR_Manager.instance.Player.LeftController;
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
#if SDK_OCULUS
            if (playerControllerEnabled)
            {
                OVRManager.display.RecenteredPose -= ResetOrientation;

                if (CameraRig != null)
                {
                    CameraRig.UpdatedAnchors -= UpdateTransform;
                }
                playerControllerEnabled = false;
            }
#endif
        }
        float speed = 0.0f;
        void Update()
        {
            if (StopSimulation)
            {
                lastPosition = transform.position;
                FallSpeed = 0.0f;
                return;
            }

            speed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

#if SDK_OCULUS
            if (!playerControllerEnabled)
            {
                if (OVRManager.OVRManagerinitialized)
                {
                    OVRManager.display.RecenteredPose += ResetOrientation;

                    if (CameraRig != null)
                    {
                        CameraRig.UpdatedAnchors += UpdateTransform;
                    }
                    playerControllerEnabled = true;
                }
                else
                    return;
            }
#endif
            #if !UNITY_XR
            //Use keys to ratchet rotation
            if (Input.GetKeyDown(KeyCode.Q))
                buttonRotation -= RotationRatchet;

            if (Input.GetKeyDown(KeyCode.E))
                buttonRotation += RotationRatchet;
            #endif

#if SDK_OCULUS
            UpdateController();
#endif
        }

#if SDK_STEAM_VR || UNITY_XR
        private void LateUpdate()
        {
            UpdateController();
        }
#endif

        private Vector3 moveDirection;
        protected virtual void UpdateController()
        {
#if SDK_OCULUS
            if (useProfileData)
            {
                if (InitialPose == null)
                {
                    // Save the initial pose so it can be recovered if useProfileData
                    // is turned off later.
                    InitialPose = new OVRPose()
                    {
                        position = CameraRig.transform.localPosition,
                        orientation = CameraRig.transform.localRotation
                    };
                }

                var p = CameraRig.transform.localPosition;
                if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.EyeLevel)
                {
                    p.y = OVRManager.profile.eyeHeight - (0.5f * Controller.height) + Controller.center.y;
                }
                else if (OVRManager.instance.trackingOriginType == OVRManager.TrackingOrigin.FloorLevel)
                {
                    p.y = -(0.5f * Controller.height) + Controller.center.y;
                }
                CameraRig.transform.localPosition = p;
            }
            else if (InitialPose != null)
            {
                // Return to the initial pose if useProfileData was turned off at runtime
                CameraRig.transform.localPosition = InitialPose.Value.position;
                CameraRig.transform.localRotation = InitialPose.Value.orientation;
                InitialPose = null;
            }

            CameraHeight = CameraRig.centerEyeAnchor.localPosition.y;
#endif

            if (CameraUpdated != null)
            {
                CameraUpdated();
            }

            UpdateMovement();

            moveDirection = Vector3.zero;

            float motorDamp = (1.0f + (Damping * SimulationRate * Time.deltaTime));

            MoveThrottle.x /= motorDamp;
            MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
            MoveThrottle.z /= motorDamp;

            moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;

            // Gravity
            if (Controller.isGrounded && FallSpeed <= 0)
                FallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));
            else
                FallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * SimulationRate * Time.deltaTime);

            moveDirection.y += FallSpeed * SimulationRate * Time.deltaTime;


            if (Controller.isGrounded && MoveThrottle.y <= transform.lossyScale.y * 0.001f)
            {
                // Offset correction for uneven ground
                float bumpUpOffset = Mathf.Max(Controller.stepOffset, new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
                moveDirection -= bumpUpOffset * Vector3.up;
            }

            if (PreCharacterMove != null)
            {
                PreCharacterMove();
                Teleported = false;
            }

            Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), new Vector3(1, 0, 1));

            // Move contoller
            lastSpeed = moveDirection;
            Controller.Move(moveDirection);
            Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));

            if (predictedXZ != actualXZ)
                MoveThrottle += (actualXZ - predictedXZ) / (SimulationRate * Time.deltaTime);
        }

        public Vector3 lastSpeed;



        public virtual void UpdateMovement()
        {
#if SDK_OCULUS
            if (HaltUpdateMovement)
                return;
#endif

            if (EnableLinearMovement)
            {
                bool moveForward = false;
                bool moveLeft = false;
                bool moveRight = false;
                bool moveBack = false;
                
                
                #if !UNITY_XR
                moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
                moveLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
                moveRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
                moveBack = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
                #endif
                bool dpad_move = false;

                MoveScale = 1.0f;

                if ((moveForward && moveLeft) || (moveForward && moveRight) || (moveBack && moveLeft) || (moveBack && moveRight))
                    MoveScale = 0.70710678f;

                // No positional movement if we are in the air
                if (!Controller.isGrounded)
                    MoveScale *= AirSpeedControl;

                MoveScale *= SimulationRate * Time.deltaTime;

                // Compute this for key movement
                moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

                // Run!
#if !UNITY_XR
                if (dpad_move || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    moveInfluence *= 2.0f;
#endif

                Quaternion ort = forwardTransform.rotation;
                Vector3 ortEuler = ort.eulerAngles;
                ortEuler.z = ortEuler.x = 0f;
                ort = Quaternion.Euler(ortEuler);

                if (moveForward)
                    MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * Vector3.forward);
                if (moveBack)
                    MoveThrottle += ort * (transform.lossyScale.z * moveInfluence * BackAndSideDampen * Vector3.back);
                if (moveLeft)
                    MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.left);
                if (moveRight)
                    MoveThrottle += ort * (transform.lossyScale.x * moveInfluence * BackAndSideDampen * Vector3.right);



                moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

#if !UNITY_ANDROID // LeftTrigger not avail on Android game pad
                moveInfluence *= 1.0f + (leftController.Input.GetButtonDown(VR_InputButton.TumbstickPress) ? 1.0f : 0.0f);
#endif

                Vector2 primaryAxis = leftController.enabled? leftController.Input.GetJoystick() : Vector2.zero;
                //Vector2 primaryAxis = new Vector2( Input.GetAxis("Horizontal") , Input.GetAxis("Vertical") );

                if (primaryAxis.magnitude < 0.1f)
                {
                    primaryAxis = Vector2.zero;
                    moveInfluence = 0.0f;
                }

                // If speed quantization is enabled, adjust the input to the number of fixed speed steps.
                if (FixedSpeedSteps > 0)
                {
                    primaryAxis.y = Mathf.Round(primaryAxis.y * FixedSpeedSteps) / FixedSpeedSteps;
                    primaryAxis.x = Mathf.Round(primaryAxis.x * FixedSpeedSteps) / FixedSpeedSteps;
                }

                if (primaryAxis.y > 0.0f)
                    MoveThrottle += ort * (primaryAxis.y * transform.lossyScale.z * moveInfluence * Vector3.forward);

                if (primaryAxis.y < 0.0f)
                    MoveThrottle += ort * (Mathf.Abs(primaryAxis.y) * transform.lossyScale.z * moveInfluence *
                                           BackAndSideDampen * Vector3.back);

                if (primaryAxis.x < 0.0f)
                    MoveThrottle += ort * (Mathf.Abs(primaryAxis.x) * transform.lossyScale.x * moveInfluence *
                                           BackAndSideDampen * Vector3.left);

                if (primaryAxis.x > 0.0f)
                    MoveThrottle += ort * (primaryAxis.x * transform.lossyScale.x * moveInfluence * BackAndSideDampen *
                                           Vector3.right);
            }

            if (rightController.Input.GetButtonDown(VR_InputButton.Primary))
                Jump();

            if (EnableRotation)
            {
                Vector3 euler = transform.rotation.eulerAngles;
                float rotateInfluence = SimulationRate * Time.deltaTime * RotationAmount * RotationScaleMultiplier;

                euler.y += buttonRotation;
                buttonRotation = 0f;


#if !UNITY_ANDROID || UNITY_EDITOR
                if (!SkipMouseRotation)
                    euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
#endif

                if (SnapRotation)
                {
                    if (rightController.Input.GetJoystick().x < -0.25f)
                    {
                        if (ReadyToSnapTurn)
                        {
                            euler.y -= RotationRatchet;
                            ReadyToSnapTurn = false;
                        }
                    }
                    else if (rightController.Input.GetJoystick().x > 0.25f)
                    {
                        if (ReadyToSnapTurn)
                        {
                            euler.y += RotationRatchet;
                            ReadyToSnapTurn = false;
                        }
                    }
                    else
                    {
                        ReadyToSnapTurn = true;
                    }
                }
                else
                {
                    Vector2 secondaryAxis = rightController.Input.GetJoystick();
                    euler.y += secondaryAxis.x * rotateInfluence;
                }

                //bool rotationChange = transform.rotation.eulerAngles != euler;

                Vector3 previusFordward = transform.forward;


                transform.rotation = Quaternion.Euler(euler);


                Vector3 currentForward = transform.forward;


                float angle = Vector2.SignedAngle(new Vector2(currentForward.x, currentForward.z), new Vector2(previusFordward.x, previusFordward.z));

                if (Math.Abs(angle) > 0.0f)
                {
                    onPlayerRotation.Invoke(angle);
                }



            }
        }


#if SDK_OCULUS
        /// <summary>
        /// Invoked by OVRCameraRig's UpdatedAnchors callback. Allows the Hmd rotation to update the facing direction of the player.
        /// </summary>
        public void UpdateTransform(OVRCameraRig rig)
        {
            Transform root = CameraRig.trackingSpace;
            Transform centerEye = CameraRig.centerEyeAnchor;

            if (HmdRotatesY && !Teleported)
            {
                Vector3 prevPos = root.position;
                Quaternion prevRot = root.rotation;

                //transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

                root.position = prevPos;
                root.rotation = prevRot;
            }

            //UpdateController();
            if (TransformUpdated != null)
            {
                TransformUpdated(root);
            }
        }
#endif

        /// <summary>
        /// Jump! Must be enabled manually.
        /// </summary>
        public bool Jump()
        {
            if (!Controller.isGrounded)
                return false;

            MoveThrottle += new Vector3(0, transform.lossyScale.y * JumpForce, 0);

            return true;
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop()
        {
            Controller.Move(Vector3.zero);
            MoveThrottle = Vector3.zero;
            FallSpeed = 0.0f;
        }

        /// <summary>
        /// Gets the move scale multiplier.
        /// </summary>
        /// <param name="moveScaleMultiplier">Move scale multiplier.</param>
        public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
        {
            moveScaleMultiplier = MoveScaleMultiplier;
        }

        /// <summary>
        /// Sets the move scale multiplier.
        /// </summary>
        /// <param name="moveScaleMultiplier">Move scale multiplier.</param>
        public void SetMoveScaleMultiplier(float moveScaleMultiplier)
        {
            MoveScaleMultiplier = moveScaleMultiplier;
        }

        /// <summary>
        /// Gets the rotation scale multiplier.
        /// </summary>
        /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
        public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
        {
            rotationScaleMultiplier = RotationScaleMultiplier;
        }

        /// <summary>
        /// Sets the rotation scale multiplier.
        /// </summary>
        /// <param name="rotationScaleMultiplier">Rotation scale multiplier.</param>
        public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
        {
            RotationScaleMultiplier = rotationScaleMultiplier;
        }

        /// <summary>
        /// Gets the allow mouse rotation.
        /// </summary>
        /// <param name="skipMouseRotation">Allow mouse rotation.</param>
        public void GetSkipMouseRotation(ref bool skipMouseRotation)
        {
            skipMouseRotation = SkipMouseRotation;
        }

        /// <summary>
        /// Sets the allow mouse rotation.
        /// </summary>
        /// <param name="skipMouseRotation">If set to <c>true</c> allow mouse rotation.</param>
        public void SetSkipMouseRotation(bool skipMouseRotation)
        {
            SkipMouseRotation = skipMouseRotation;
        }

        public void DisableGravity()
        {
            GravityModifier = 0.0f;
        }

        public void ResetGravity()
        {
            GravityModifier = originalGravity;
        }

        public void Disable()
        {
            enabled = false;
        }

        public void Enable()
        {
            enabled = true;
        }


#if SDK_OCULUS
        /// <summary>
        /// Gets the halt update movement.
        /// </summary>
        /// <param name="haltUpdateMovement">Halt update movement.</param>
        public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
        {
            haltUpdateMovement = HaltUpdateMovement;
        }

        /// <summary>
        /// Sets the halt update movement.
        /// </summary>
        /// <param name="haltUpdateMovement">If set to <c>true</c> halt update movement.</param>
        public void SetHaltUpdateMovement(bool haltUpdateMovement)
        {
            HaltUpdateMovement = haltUpdateMovement;
        }

        public void ResetOrientation()
        {
            if (HmdResetsY && !HmdRotatesY)
            {
                Vector3 euler = transform.rotation.eulerAngles;
                euler.y = InitialYRotation;
                transform.rotation = Quaternion.Euler( euler );
            }
        }
#endif
    }

}

