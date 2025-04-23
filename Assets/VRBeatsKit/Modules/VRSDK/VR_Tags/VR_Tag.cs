using UnityEngine;

namespace VRSDK
{
    public class VR_Tag : MonoBehaviour
    {
        [SerializeField] private VR_TagsEnum tagEnum;
        public VR_TagsEnum TagEnum { get { return tagEnum; } }
    }

}

