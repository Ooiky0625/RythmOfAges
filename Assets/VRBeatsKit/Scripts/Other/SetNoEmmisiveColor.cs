using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBeats
{
    public class SetNoEmmisiveColor : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderArray = null;
        [SerializeField] private VR_BeatCubeSpawneable spanwneable = null;

        private Color SideColor 
        {
            get
            {
                if (spanwneable.ColorSide == ColorSide.Right)
                    return VR_BeatManager.instance.GameSettings.RightColor;
                return VR_BeatManager.instance.GameSettings.LeftColor;
            }
        }

        public void SetColor()
        {
            for (int n = 0; n < renderArray.Length; n++)
            {
                Material[] materialArray = renderArray[n].materials;

                for (int j = 0; j < materialArray.Length; j++)
                {
                    materialArray[j].SetColor("_BaseColor", SideColor);
                }

                renderArray[n].materials = materialArray;

            }
        }
    }

}

