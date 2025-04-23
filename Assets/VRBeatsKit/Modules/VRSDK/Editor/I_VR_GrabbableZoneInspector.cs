using Platinio.SDK.EditorTools;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRSDK.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( VR_GrabbableZone ) )]
    public class I_VR_GrabbableZoneInspector : Editor
    {

        private SerializedProperty interactDistance = null;
        private SerializedProperty interactButton = null;
        private SerializedProperty usePerHandSettings = null;
        private SerializedProperty leftHandSettings = null;
        private SerializedProperty handSettings = null;
        private SerializedProperty rightHandSettings = null;
        private SerializedProperty grabbable = null;        
        private VR_GrabbableZone targetScript = null;

        private void OnEnable()
        {
            interactDistance = serializedObject.FindProperty( "interactDistance" );
            interactButton = serializedObject.FindProperty( "interactButton" );
            usePerHandSettings = serializedObject.FindProperty( "usePerHandSettings" );
            rightHandSettings = serializedObject.FindProperty( "rightHandSettings" );
            leftHandSettings = serializedObject.FindProperty( "leftHandSettings" );
            handSettings = serializedObject.FindProperty( "handSettings" );            
            grabbable = serializedObject.FindProperty("grabbable");            

            targetScript = (VR_GrabbableZone) target;
        }

        public override void OnInspectorGUI()
        {
            PlatinioEditorGUILayout.PropertyField( grabbable );
            PlatinioEditorGUILayout.PropertyField( interactDistance );
          
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHandSettings(SerializedProperty settings)
        {
            EditorGUILayout.PropertyField( settings, true );
        }


        public void OnSceneGUI()
        {/*
            if (targetScript == null)
                OnEnable();


            EditorGUI.BeginChangeCheck();

            Handles.color = Color.blue;

           
            if (usePerHandSettings.boolValue)
            {
                float rightValue = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointRightHand == null ? targetScript.transform.position : targetScript.HighlightPointRightHand.position, interactDistance.floatValue );
                float leftValue = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointLeftHand == null ? targetScript.transform.position : targetScript.HighlightPointLeftHand.position, interactDistance.floatValue );

                UpdateGrabDistanceValue( rightValue, leftValue );
            }
            else
            {
                if (targetScript.HighlightPointHandSettings == null)
                    return;

                float value = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointHandSettings.position, interactDistance.floatValue );
               
                targetScript.Grabbable.SetInteractDistanceViaInspector( value);
                EditorUtility.SetDirty( targetScript );
                //EditorSceneManager.MarkAllScenesDirty();
            }
            */

            
        }

        private void UpdateGrabDistanceValue(float rightValue, float leftValue)
        {
            if (interactDistance.floatValue != rightValue)
            {
                targetScript.Grabbable.SetInteractDistanceViaInspector( rightValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
            else if (interactDistance.floatValue != leftValue)
            {
                targetScript.Grabbable.SetInteractDistanceViaInspector( leftValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }

}

