using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Platinio;



namespace VRSDK
{
    

    public enum Axis
    {
        Horizontal,
        Vertical,
        Forward
    }

    public enum VR_SDK
    {
        None,
        Oculus,
        Steam_VR,
        UnityXR
    }


    public class VR_Manager : Singleton<VR_Manager>
    {
        #region INSPECTOR
        [SerializeField] private VR_SDK currentSDK = VR_SDK.None;        
        [SerializeField] private ControllerGestureConfig gestureConfig = null;        
        #endregion

        #region PUBLIC
        public VR_SDK CurrentSDK { get { return currentSDK; } }
        public List<VR_Interactable> InteractList { get { return interactList; } }
        public List<VR_Highlight> HighlightList { get { return highlightList; } }
        public List<VR_Grabbable> GrabbableList { get { return grabbableList; } }
        public VR_Player Player
        {
            get
            {
                if (player == null)
                {
                    player = FindObjectOfType<VR_Player>();
                    player.Construct();
                }

                return player;
            }
        }
        #endregion

        #region PRIVATE
        private List<VR_Interactable> interactList = new List<VR_Interactable>();
        private List<VR_Highlight> highlightList = new List<VR_Highlight>();
        private List<VR_Grabbable> grabbableList = new List<VR_Grabbable>();
        private VR_Player player = null;
        #endregion


        protected override void Awake()
        {
            m_destroyOnLoad = false;
            base.Awake();
        }

        public void SetCurrentPlayer(VR_Player player)
        {
            player.Construct();
        }


        /// <summary>
        /// Register a grabbable object
        /// </summary>
        /// <param name="grabbable"></param>
        public void RegisterInteract(VR_Interactable interact)
        {
            if (interactList.Contains( interact ))
                return;

            interactList.Add( interact );

            //is this is a grabbable to lets have it on a diferent list
            if (interact is VR_Grabbable)
                grabbableList.Add( interact as VR_Grabbable );
        }

        /// <summary>
        /// Remove a grabbable object
        /// </summary>
        /// <param name="grabbable"></param>
        public void RemoveInteract(VR_Interactable interact)
        {
            interactList.Remove( interact );

            //if this is a grabbable to remove it from grabbable list
            if (interact is VR_Grabbable)
                grabbableList.Remove( interact as VR_Grabbable );
        }

        public void RegisterHighlight(VR_Highlight h)
        {
            highlightList.Add( h );
        }

        public void RemoveHighlight(VR_Highlight h)
        {
            highlightList.Remove( h );
        }

        public VR_Grabbable GetGrabbableFromCollider(Collider c)
        {
            for (int n = 0; n < interactList.Count; n++)
            {
                VR_Grabbable grabbable = interactList[n] as VR_Grabbable;

                if (grabbable != null && grabbable.ColliderList != null && grabbable.ColliderList.Count > 0 && grabbable.ColliderList.Contains( c ))
                {
                    return grabbable;
                }
            }

            return null;
        }

        public void SetCurrentSDKViaEditor(VR_SDK sdk)
        {
            currentSDK = sdk;
        }
    }

    [System.Serializable]
    public class ControllerGestureConfig
    {
        public float minAcelerationThreshold = 15.0f;
        public float maxAcelerationThreshold = 40.0f;       
    }

}

