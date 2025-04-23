using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace VRBeats
{
    [HideInMenu]
    public class VR_BeatMarker : Marker , INotification
    {
        public PropertyName id { get { return new PropertyName(); } }
    }


}
