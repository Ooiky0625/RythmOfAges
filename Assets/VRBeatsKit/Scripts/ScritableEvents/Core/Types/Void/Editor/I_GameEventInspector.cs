using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace VRBeats.ScriptableEvents
{
    [CustomEditor(typeof(GameEvent))]
    public class I_GameEventInspector : Editor
    {
        private GameEvent targetEvent = null;

        private void OnEnable()
        {
            targetEvent = (GameEvent)target;
            
        }


        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Sritable events can only be use in running time" , MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Listeners" );
            for (int n = 0; n < targetEvent.EventListenerList.Count; n++)
            {
                if (GUILayout.Button(targetEvent.EventListenerList[n].gameObject.name))
                {
                    EditorGUIUtility.PingObject(targetEvent.EventListenerList[n].gameObject);
                }

                DrawEventNames(targetEvent.EventListenerList[n].Response );

            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Invoke"))
            {
                targetEvent.Invoke();
            }

        }

        private void DrawEventNames(UnityEvent unityEvent)
        {
            for (int n = 0; n < unityEvent.GetPersistentEventCount(); n++)
            {                
                EditorGUILayout.LabelField( unityEvent.GetPersistentTarget(n).GetType().Name + "." +unityEvent.GetPersistentMethodName(n) );
            }
        }

    }
}
