using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBeats
{
    public static class GameObjectExtension
    {
        /// <summary>
        /// VR Shooter Kit extension method just try to get a component, if the component is no found we add one
        /// </summary>       
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();

            if (component == null)
                component = go.AddComponent<T>();

            return component;
        }
    }

}

