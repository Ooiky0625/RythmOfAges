using UnityEngine;

namespace DamageSystem
{
    public class GlobalSurfaceDetails : MonoBehaviour
    {
        [SerializeField] private SurfaceDetails copyBase = null;

        private void Awake()
        {
            SurfaceDetails[] surfaceDetailsArray = transform.GetComponentsInChildren<SurfaceDetails>();

            for (int n = 0; n < surfaceDetailsArray.Length; n++)
            {
                surfaceDetailsArray[n].CopySettings(copyBase);
            }
        }
    }
}

