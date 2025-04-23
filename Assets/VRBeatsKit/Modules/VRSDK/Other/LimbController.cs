using UnityEngine;

namespace VRSDK
{
    public class LimbController : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 5.0f;

        private VR_Grabbable grabbable = null;
        private float timer = 0.0f;

        private void Awake()
        {
            grabbable = GetComponent<VR_Grabbable>();
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer > lifeTime)
            {
                Destroy( gameObject );
            }
            //dont destroy while you are grabbing this limb
            else if( grabbable.CurrentGrabState == GrabState.Grab )
            {
                timer = 0.0f;
            }
        }


    }
}

