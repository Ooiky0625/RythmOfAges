using UnityEngine.Events;

namespace VRSDK.Events
{
    /// <summary>
    /// Event that gets called when a rotation gesture occurs, we extend from UnityEvent<GrabState> so we can see this onn the inspector and add some listeners by hand if we want
    /// this is currently in use in the revolver Physical reload system, when we rotate our hand a certain speed and certain angle, the weapon gets a reload
    /// </summary>
    public class OnRotationGestureEvent : UnityEvent<RotationGestureInfo> { }
}

