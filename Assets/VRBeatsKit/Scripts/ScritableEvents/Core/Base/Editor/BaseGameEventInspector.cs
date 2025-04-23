using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace VRBeats.ScriptableEvents
{
    //T1 IntGameEvent
    //T2 int
    //T3 IntEventListener
    //T4 OnIntValueChange

    public class BaseGameEventInspector<T1 , T2 , T3, T4> : Editor 
    where T1 : BaseGameEvent<T3 , T2> 
    where T3 : BaseEventListener<T1 , T2 , T3 , T4>
    where T4 : UnityEvent<T2>
    {
        
        private T1 targetEvent = null;
        private SerializedProperty invokeValue;


        private void OnEnable()
        {
            targetEvent = (T1) target;
            invokeValue = serializedObject.FindProperty("invokeValue");
        }


        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Sritable events can only be use in running time", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Listeners");
            for (int n = 0; n < targetEvent.EventListenerList.Count; n++)
            {
                if (GUILayout.Button(targetEvent.EventListenerList[n].gameObject.name))
                {
                    EditorGUIUtility.PingObject(targetEvent.EventListenerList[n].gameObject);
                }

                DrawEventNames(targetEvent.EventListenerList[n].Response);

            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(invokeValue);

            if (GUILayout.Button("Invoke"))
            {
                targetEvent.Invoke( targetEvent.invokeValue);
            }

            serializedObject.ApplyModifiedProperties();

        }

        private void DrawEventNames(UnityEvent<T2> unityEvent)
        {
            for (int n = 0; n < unityEvent.GetPersistentEventCount(); n++)
            {
                EditorGUILayout.LabelField(unityEvent.GetPersistentTarget(n).GetType().Name + "." + unityEvent.GetPersistentMethodName(n));
            }
        }

    }
}
