using UnityEngine;
using System.Collections.Generic;

namespace VRSDK
{
    public class VR_ColorHiglight : VR_Highlight
    {
        [SerializeField] private Color highlightColor = Color.black; 

        private MeshRenderer[] meshRenderArray = null;
        private Dictionary<MeshRenderer, Color[]> meshRenderDic = new Dictionary<MeshRenderer, Color[]>();

        protected override void Awake()
        {
            base.Awake();

            meshRenderArray = GetComponentsInChildren<MeshRenderer>();

            for (int j = 0; j < meshRenderArray.Length; j++)
            {
                
                Color[] colorArray = new Color[ meshRenderArray[j].materials.Length ];

                for (int k = 0; k < meshRenderArray[j].materials.Length; k++)
                {
                    colorArray[k] = meshRenderArray[j].materials[k].color;
                }

                meshRenderDic[meshRenderArray[j]] = colorArray;

            }
            
        }

        public override void Highlight(VR_Controller controller)
        {
            base.Highlight( controller );

            OverrideColor( highlightColor );
        }

        private void OverrideColor(Color c)
        {
            for (int j = 0; j < meshRenderArray.Length; j++)
            {
                if (meshRenderArray[j] != null)
                {
                    for (int k = 0; k < meshRenderArray[j].materials.Length; k++)
                    {
                        meshRenderArray[j].materials[k].color = c;
                    }
                }                
            }
        }

        public override void UnHighlight(VR_Controller controller)
        {
            base.UnHighlight( controller );

            SetOriginalColor();
        }

        private void SetOriginalColor()
        {
            for (int j = 0; j < meshRenderArray.Length; j++)
            {
                if (meshRenderArray[j] != null)
                {
                    for (int k = 0; k < meshRenderArray[j].materials.Length; k++)
                    {
                        meshRenderArray[j].materials[k].color = meshRenderDic[meshRenderArray[j]][k];
                    }
                }                
            }
        }



    }
}

