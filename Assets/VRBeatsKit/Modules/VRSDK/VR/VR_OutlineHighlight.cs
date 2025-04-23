using UnityEngine;

namespace VRSDK
{
    [RequireComponent( typeof( VR_Outline ) )]
    public class VR_OutlineHighlight : VR_Highlight
    {
        private VR_Outline outline = null;

        protected override void Awake()
        {
            base.Awake();
            outline = GetComponent<VR_Outline>();
        }

        public override void Highlight(VR_Controller controller)
        {
            base.Highlight( controller );
            outline.outlineActive = 1.0f;
        }

        public override void UnHighlight(VR_Controller controller)
        {
            base.UnHighlight( controller );
           
            outline.outlineActive = 0.0f;
        }
    }

}
