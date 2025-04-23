using UnityEngine;
using VRSDK;

namespace VRBeats
{
    public class VR_Saber : MonoBehaviour
    {
        [SerializeField] private MaterialBindings[] materialBindingArray = null;
        [SerializeField] private float emmisionIntensity = 10.0f;
        [SerializeField] private Transform body = null;

        public Transform Body { get { return body; } }
        public ColorSide ColorSide { get { return colorSide; } }

        private VR_Grabbable grabbable = null;
        private ColorSide colorSide = ColorSide.Left;
        private MeshRenderer[] renderArray = null;

        private void Awake()
        {
            renderArray = transform.GetComponentsInChildren<MeshRenderer>();

            grabbable = GetComponent<VR_Grabbable>();
            grabbable.OnGrabStateChange.AddListener(OnGrabStateChange);
             
        }

        private void OnGrabStateChange(GrabState state)
        {
            if (state == GrabState.Grab)
            {
                VR_ControllerType controllerType = grabbable.GrabController.ControllerType;
                ColorSide colorSide = controllerType == VR_ControllerType.Right ? ColorSide.Right : ColorSide.Left;
                Construct(colorSide);
            }
        }


        public void Construct(ColorSide colorSide)
        {
            this.colorSide = colorSide;
            Color c = colorSide == ColorSide.Right ? VR_BeatManager.instance.RightColor : VR_BeatManager.instance.LeftColor;
            for (int n = 0; n < materialBindingArray.Length; n++)
            {
                SetMaterialBindings(materialBindingArray[n], c);
            }
        }

        private void SetMaterialBindings(MaterialBindings matBindings, Color c)
        {
            matBindings.SetUseEmmisiveIntensity(false);
            matBindings.SetEmmisiveColor(c * emmisionIntensity);
        }

        public void MakeVisible()
        {
            SetRenderArrayEnableValue(true);
        }

        public void MakeInvisible()
        {
            SetRenderArrayEnableValue(false);
        }

        private void SetRenderArrayEnableValue(bool value)
        {
            for (int n = 0;n < renderArray.Length; n++)
            {
                renderArray[n].enabled = value;
            }
        }

    }

}