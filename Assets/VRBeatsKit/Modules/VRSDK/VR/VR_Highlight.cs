using UnityEngine;

namespace VRSDK
{
    //this is the basic class for highlight grabbables
    //in used by the VR_OutlineHighlight and the VR_UIHighlight
    public abstract class VR_Highlight : MonoBehaviour
    {
        private VR_Interactable interact = null;

        public Transform HighlightPointRightHand { get { return interact.HighlightPointRightHand; } }
        public Transform HighlightPointLeftHand { get { return interact.HighlightPointLeftHand; } }
        public float HighlightDistance { get { return interact.InteractDistance; } }
        public bool IsHighlight { get; private set; }

        public VR_Interactable Interactable
        {
            get { return interact; }
        }

        protected virtual void Awake()
        {
            interact = GetComponent<VR_Interactable>();

            if (interact == null)
            {
                Debug.LogError( "VR_Hightlight attached to " + gameObject.name + " needs a interactable script in order to work" );
            }

            VR_Manager.instance.RegisterHighlight( this );
        }

        protected virtual void OnDestroy()
        {
            //this object can be destroyed for 2 reasons
            //the programmer calling destroy on the gameobject
            // and the UnityEngine closing the game, so if Unity is closing the game
            //dont do nothing the game is just closing
            if (!VR_Manager.ApplicationIsQuitting)
                VR_Manager.instance.RemoveHighlight( this );
        }

        protected virtual void OnEnable()
        {
            VR_Manager.instance.RegisterHighlight( this );
        }

        protected virtual void OnDisable()
        {
            //this object can be disable for 2 reasons
            //the programmer calling destroy on the gameobject
            // and the UnityEngine closing the game, so if Unity is closing the game
            //dont do nothing the game is just closing
            if (!VR_Manager.ApplicationIsQuitting)
                VR_Manager.instance.RemoveHighlight( this );
        }

        public bool CanHighlight()
        {
            return interact == null || interact.CanInteract;
        }

        public bool CanHighlightUsingController(VR_Controller controller)
        {
            return interact == null || interact.CanInteractUsingController( controller );
        }


        public virtual void Highlight(VR_Controller controller)
        {
            IsHighlight = true;
        }
        public virtual void UnHighlight(VR_Controller controller)
        {
            IsHighlight = false;
        }

    }
}

