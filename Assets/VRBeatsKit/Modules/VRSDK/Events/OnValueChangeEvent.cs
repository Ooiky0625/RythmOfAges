using UnityEngine.Events;

namespace VRSDK.Events
{
    /// <summary>
    /// Event that gets called when some valud change, we extend from UnityEvent<GrabState> so we can see this onn the inspector and add some listeners by hand if we want
    /// </summary>
    [System.Serializable]
    public class OnValueChangeEvent : UnityEvent<float> { }
}