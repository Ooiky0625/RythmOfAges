namespace VRSDK
{
    public enum GrabSelectionMenu
    {
        Animation       = 0,
        GrabSettings    = 1,
        Other           = 2,
        Events          = 3
    }

    public enum BoolEnum
    {
        Yes,
        No
    }

    [System.Serializable]
    public class VR_GrabbableEditorPart
    {
        public GrabSelectionMenu selectedMenu = GrabSelectionMenu.Animation;     
        
        public bool foldoutInput = false;
        public bool foldoutConsole = false;
        public bool foldoutBaseInspector = false;
        public bool foldoutBasic = false;
        public bool foldoutInteraction = false;
        public bool foldoutShareHandInteractSettings = false;
        public bool foldoutRightHandInteractSettings = false;
        public bool foldoutLeftHandInteractSettings = false;
        public bool foldoutShareHandAnimationSettings = false;
        public bool foldoutRightHandAnimationSettings = false;
        public bool foldoutLeftHandAnimationSettings = false;
        public bool foldoutEditorTools = false;

        public int handSelected = 0;

                
        public bool IsMissingGrabPoint(VR_HandInteractSettings settings)
        {            
            return settings.canInteract && settings.interactPoint == null;
        }

        public bool IsMissinHiglightPoint(VR_HandInteractSettings settings)
        {
            return settings.canInteract && settings.highlightPoint == null;
        }

    }
}

