using Platinio.SDK.EditorTools;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRSDK.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_Interactable))]
    public class I_VR_InteractableInspector : Editor
    {
        
        private SerializedProperty interactDistance = null;
        private SerializedProperty interactButton = null;
        private SerializedProperty usePerHandSettings = null;
        private SerializedProperty leftHandSettings = null;
        private SerializedProperty handSettings = null;
        private SerializedProperty rightHandSettings = null;
        private SerializedProperty onInteractEvent = null;

        private VR_Interactable targetScript = null;

        private void OnEnable()
        {
            interactDistance = serializedObject.FindProperty("interactDistance");
            interactButton = serializedObject.FindProperty("interactButton");
            usePerHandSettings = serializedObject.FindProperty( "usePerHandSettings" );
            rightHandSettings = serializedObject.FindProperty( "rightHandSettings" );
            leftHandSettings = serializedObject.FindProperty( "leftHandSettings" );
            handSettings = serializedObject.FindProperty( "handSettings" );
            onInteractEvent = serializedObject.FindProperty( "onInteractEvent" );

            targetScript = (VR_Interactable) target;
        }

        public override void OnInspectorGUI()
        {
            PlatinioEditorGUILayout.PropertyField( interactDistance );
            PlatinioEditorGUILayout.PropertyField( usePerHandSettings );

            if (usePerHandSettings.boolValue)
            {
                EditorGUILayout.PropertyField( rightHandSettings, true );
                EditorGUILayout.PropertyField( leftHandSettings, true );
            }
            else
            {
                EditorGUILayout.PropertyField( handSettings, true );
            }

            PlatinioEditorGUILayout.PropertyField( interactButton );
            PlatinioEditorGUILayout.PropertyField( onInteractEvent );
          
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHandSettings(SerializedProperty settings)
        {
            EditorGUILayout.PropertyField( settings, true );
        }


        public void OnSceneGUI()
        {
            if (targetScript == null)
                OnEnable();

            //handler for interact distance
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
                interactDistance.floatValue = value;
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateGrabDistanceValue(float rightValue, float leftValue)
        {
            if (interactDistance.floatValue != rightValue)
            {
                targetScript.SetInteractDistanceViaInspector( rightValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
            else if (interactDistance.floatValue != leftValue)
            {
                targetScript.SetInteractDistanceViaInspector( leftValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }

}

