using UnityEngine;
using System.Collections.Generic;
using VRSDK.Events;

namespace VRSDK
{
    public enum InteractableType
    {
        Distance,
        Collider
    }

    //basic class for all interactable components
    public class VR_Interactable : MonoBehaviour
    {
        [SerializeField] protected InteractableType interactableType = InteractableType.Distance;
        [SerializeField] protected VR_InputButton interactButton = VR_InputButton.Grip;
        [SerializeField] protected float interactDistance = 1.0f;
        [SerializeField] protected Collider grabCollider = null;
        [SerializeField] protected VR_HandInteractSettings rightHandSettings = new VR_HandInteractSettings();
        [SerializeField] protected VR_HandInteractSettings leftHandSettings = new VR_HandInteractSettings();
        [SerializeField] protected VR_HandInteractSettings handSettings = new VR_HandInteractSettings();
        [SerializeField] protected VR_HandAnimationSettings rightHandAnimationSettings = new VR_HandAnimationSettings();
        [SerializeField] protected VR_HandAnimationSettings leftHandAnimationSettings = new VR_HandAnimationSettings();
        [SerializeField] protected VR_HandAnimationSettings handAnimationSettings = new VR_HandAnimationSettings();
        [SerializeField] protected OnInteractEvent onInteractEvent = null;
        [SerializeField] protected bool shareHandInteractionSettings = false;
        [SerializeField] protected bool shareHandAnimationSettings = false;
        [SerializeField] protected bool useDistanceGrab = false;


        public bool UseDistanceGrab { get { return useDistanceGrab; } }
        public float DistanceToLeftHand { get; private set; }
        public float DistanceToRightHand { get; private set; }
        public float InteractDistance { get { return interactDistance; } }
        public bool CanInteract
        {
            get
            {
                return canInteract;
            }
            protected set
            {
                canInteract = value;

                if (!canInteract)
                {
                    VR_Highlight h = GetComponent<VR_Highlight>();

                    if (h != null)
                        h.UnHighlight(null);
                }
            }
        }
        public OnInteractEvent OnInteractEvent { get { return onInteractEvent; } }
        public InteractableType InteractableType => interactableType;

        protected bool m_buttonWasPressedLeft = false;
        protected bool m_buttonWasPressedRight = false;

        private bool canInteract = true;
        private VR_ControllerInfo rightControllerInfo = null;
        private VR_ControllerInfo leftControllerInfo = null;
        private List<VR_Controller> activeDistanceGrabControllerList = new List<VR_Controller>();
        private Transform cacheTransform = null;

        protected Transform thisTransform
        {
            get
            {
                if (cacheTransform == null)
                {
                    cacheTransform = transform;
                }

                return cacheTransform;
            }
        }

        public Transform HighlightPointRightHand
        {
            get
            {
                return rightHandSettings.highlightPoint == null ? rightHandSettings.interactPoint : rightHandSettings.highlightPoint;
            }
        }

        public Transform HighlightPointLeftHand
        {
            get
            {
                return leftHandSettings.highlightPoint == null ? leftHandSettings.interactPoint : leftHandSettings.highlightPoint;
            }
        }

        public Transform HighlightPointHandSettings
        {
            get
            {
                return handSettings.highlightPoint;
            }
        }

        public Transform RightInteractPoint
        {
            get
            {
                if (!shareHandInteractionSettings)
                    return rightHandSettings.interactPoint;
                else
                    return handSettings.interactPoint;
            }
        }

        public Transform LeftInteractPoint
        {
            get
            {
                if (!shareHandInteractionSettings)
                    return rightHandSettings.interactPoint;
                else
                    return handSettings.interactPoint;
            }
        }

        public VR_HandInteractSettings HandSettings { get { return handSettings; } }
        public VR_HandInteractSettings RightHandSettings { get { return rightHandSettings; } }
        public VR_HandInteractSettings LeftHandSettings { get { return leftHandSettings; } }
        public VR_HandAnimationSettings HandAnimationSettings { get { return handAnimationSettings; ; } }
        public VR_HandAnimationSettings RightHandAnimationSettings { get { return rightHandAnimationSettings; } }
        public VR_HandAnimationSettings LeftHandAnimationSettings { get { return leftHandAnimationSettings; } }
        public VR_InputButton InteractButton { get { return interactButton; } }
        public Collider GrabCollider => grabCollider;

        protected virtual void Awake()
        {

            //if we dont use per hand settings set general settings as the settings for both hands
            if (shareHandInteractionSettings)
            {
                rightHandSettings = handSettings;
                leftHandSettings = handSettings;
            }

            if (shareHandAnimationSettings)
            {
                rightHandAnimationSettings = handAnimationSettings;
                leftHandAnimationSettings = handAnimationSettings;
            }

            //register this interactable
            VR_Manager.instance.RegisterInteract(this);

            //create snap points if we need they
            CreateAllSnapPoints();
        }

        protected virtual void Start()
        {
            CreateControllersInfo();
        }

        private void CreateAllSnapPoints()
        {
            if (rightHandSettings.interactPoint == null)
                CreateSnapPoint(rightHandSettings);

            if (leftHandSettings.interactPoint == null)
                CreateSnapPoint(leftHandSettings);

            if (handSettings.interactPoint == null)
                CreateSnapPoint(handSettings);
        }

        private void CreateSnapPoint(VR_HandInteractSettings settings)
        {
            settings.interactPoint = new GameObject("SnapPoint").transform;
            settings.interactPoint.parent = transform;
            settings.interactPoint.localPosition = Vector3.zero;
            settings.interactPoint.localRotation = Quaternion.identity;
        }

        private void CreateControllersInfo()
        {
            rightControllerInfo = new VR_ControllerInfo(VR_Manager.instance.Player.RightController);
            leftControllerInfo = new VR_ControllerInfo(VR_Manager.instance.Player.LeftController);
        }


        private void OnDisable()
        {
            //this object can be destroyed for 2 reasons
            //the programmer calling destroy on the gameobject
            // and the UnityEngine closing the game, so if Unity is closing the game
            //dont do nothing the game is just closing
            if (!VR_Manager.ApplicationIsQuitting)
                VR_Manager.instance.RemoveInteract(this);
        }

        private void OnEnable()
        {
            VR_Manager.instance.RegisterInteract(this);
        }

        protected virtual void OnDestroy()
        {
            //this object can be destroyed for 2 reasons
            //the programmer calling destroy on the gameobject
            // and the UnityEngine closing the game, so if Unity is closing the game
            //dont do nothing the game is just closing
            if (!VR_Manager.ApplicationIsQuitting)
                VR_Manager.instance.RemoveInteract(this);
        }

        protected virtual void Update()
        {
            if (!CanInteract) return;

            ProcessControllerInfoInteraction(rightControllerInfo);
            ProcessControllerInfoInteraction(leftControllerInfo);
        }

        private void ProcessControllerInfoInteraction(VR_ControllerInfo info)
        {
            if (info?.controller == null)
            {
                return;
            }

            VR_HandInteractSettings settings = info.controller.ControllerType == VR_ControllerType.Right ? rightHandSettings : leftHandSettings;
            Transform highlightPoint = info.controller.ControllerType == VR_ControllerType.Right ? HighlightPointRightHand : HighlightPointLeftHand;
            bool ignoreDistance = interactableType == InteractableType.Collider;

            //if we are trying to do a distance grab using this controller
            if (!ignoreDistance && useDistanceGrab && activeDistanceGrabControllerList != null && activeDistanceGrabControllerList.Count > 0 && activeDistanceGrabControllerList.Contains(info.controller))
            {
                ignoreDistance = true;
            }

            if (settings.canInteract && highlightPoint != null)
            {
                CheckIfShouldInteractWithController(info, ignoreDistance);
            }
        }

        private void CheckIfShouldInteractWithController(VR_ControllerInfo info, bool ignoreDistance)
        {

            float d = ignoreDistance ? 0.0f : GetDistanceToController(info.controller);

            if (d <= interactDistance || ignoreDistance)
            {
                //the button was alredy pressed when the controller enter the grab range?
                if (info.interactionButtonWasPressed)
                {
                    info.UpdateButtonWasPressed(interactButton);

                    if (info.interactionButtonWasPressed)
                    {
                        return;
                    }
                }

                if (info.controller.enabled && info.controller.Input.GetButtonDown(interactButton))
                {
                    info.controller.InteractWithNearesObject();
                    info.interactionButtonWasPressed = true;
                }

            }
            else
            {
                info.UpdateButtonWasPressed(interactButton);
            }
        }


        /// <summary>
        /// Get the distance to the controller
        /// </summary>       
        private float GetDistanceToController(VR_Controller controller)
        {
            float d = float.MaxValue;
            Transform interactPoint = controller.ControllerType == VR_ControllerType.Right ? HighlightPointRightHand : HighlightPointLeftHand;

            if (controller.Input.IsConnected() && controller.CurrentGrab == null)
            {
                d = (interactPoint.position - controller.Position).magnitude;
            }

            return d;
        }

        public virtual void Interact(VR_Controller controller)
        {
            onInteractEvent.Invoke(controller);
        }

        public bool CanInteractUsingController(VR_Controller controller)
        {
            if (controller.ControllerType == VR_ControllerType.Right)
                return rightHandSettings.canInteract;
            if (controller.ControllerType == VR_ControllerType.Left)
                return leftHandSettings.canInteract;

            return false;
        }

        public VR_HandInteractSettings GetHandInteractionSettings(VR_Controller controller)
        {
            if (controller.ControllerType == VR_ControllerType.Right)
                return rightHandSettings;
            if (controller.ControllerType == VR_ControllerType.Left)
                return leftHandSettings;

            return null;
        }

        public VR_HandAnimationSettings GetHandAnimationSettings(VR_Controller controller)
        {
            if (controller.ControllerType == VR_ControllerType.Right)
                return rightHandAnimationSettings;
            if (controller.ControllerType == VR_ControllerType.Left)
                return leftHandAnimationSettings;

            return null;
        }

        public void SetInteractDistanceViaInspector(float d)
        {
            interactDistance = d;
        }

        public void AddActiveDistanceGrabController(VR_Controller controller)
        {
            if (!useDistanceGrab)
                return;

            activeDistanceGrabControllerList.Add(controller);
        }

        public void RemoveActiveDistanceGrabController(VR_Controller controller)
        {
            if (!useDistanceGrab)
                return;

            activeDistanceGrabControllerList.Remove(controller);
        }

    }

    public class VR_ControllerInfo
    {
        public VR_Controller controller = null;
        public bool interactionButtonWasPressed = false;

        public VR_ControllerInfo(VR_Controller controller)
        {
            this.controller = controller;
            interactionButtonWasPressed = false;
        }

        public void UpdateButtonWasPressed(VR_InputButton interactButton)
        {
            interactionButtonWasPressed = controller.Input.GetButtonDown(interactButton);
        }
    }

}

