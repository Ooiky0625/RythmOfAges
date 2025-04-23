using UnityEngine;
using System.Collections.Generic;

namespace VRSDK.Locomotion
{
  
    /// Class for handling rendering of a teleporting line    
    [RequireComponent(typeof(LineRenderer))]    
    public class VR_LineRenderer : MonoBehaviour
    {
        [SerializeField] private Gradient validTeleportGradient = null;
        [SerializeField] private Gradient invalidTeleportGradient = null;

        private LineRenderer lineRender = null;

        private void Awake()
        {
            lineRender = GetComponent<LineRenderer>();
        }

        public virtual void CleanRender()
        {
            lineRender.positionCount = 0;
        }

        public virtual void Render(List<Vector3> points , bool suitableForTeleporting)
        {
            lineRender.positionCount = points.Count;
            lineRender.colorGradient = suitableForTeleporting ? validTeleportGradient : invalidTeleportGradient;
            
            for(int n = 0; n < points.Count; n++)
            {
                lineRender.SetPosition(n , points[n]);
            }
        }
        
    }

}

