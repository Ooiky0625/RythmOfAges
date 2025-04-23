using UnityEngine;

namespace VRSDK
{
    public class DoorAnimatorController : MonoBehaviour
    {
        [SerializeField] private DemoScene demoScene = null;

        //called by the door animation when the door open
        public void Open()
        {
            demoScene.SetDoorState(true);
        }

        //called by the door animation when the door closes
        public void Close()
        {
            demoScene.SetDoorState(false);
        }
    }
}

