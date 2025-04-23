using UnityEngine;

namespace VRSDK
{
    //this is code for the demo wall cube
    public class WallCube : MonoBehaviour
    {
        [SerializeField] private float resetTime = 1.0f;

        private WallCubePart[] wallCubePartArray = null;

        private void Start()
        {
            wallCubePartArray = transform.GetComponentsInChildren<WallCubePart>();
        }

        public void Reset()
        {
            for (int n = 0; n < wallCubePartArray.Length; n++)
            {
                wallCubePartArray[n].Reset( resetTime );
            }
        }

    }
}

