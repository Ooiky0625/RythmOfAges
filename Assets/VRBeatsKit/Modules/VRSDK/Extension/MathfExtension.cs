using UnityEngine;

namespace VRSDK
{
    public static class MathfExtension
    {
        public static Vector3 ClampVectorMagnitude(Vector3 value , float min , float max)
        {
            Vector3 clampVector = value;
            float m = clampVector.magnitude;

            if (m > max)
            {
                clampVector = clampVector.normalized * max;
            }
            else if (m < min)
            {
                clampVector = clampVector.normalized * min;
            }

            return clampVector;
        }
       
    }
}

