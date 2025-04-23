using Platinio.SDK.EditorTools;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace VRSDK.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_Lever))]
    public class I_VR_LeverInspector : Editor
    {
        private SerializedProperty grabButton = null;
        private SerializedProperty grabDistance = null;             
        private SerializedProperty transformBase = null;
        private SerializedProperty solverIterations = null;
        private SerializedProperty shouldBackToStartingPosition = null;
        private SerializedProperty onGrabStateChange = null;
        private SerializedProperty onValueChange = null;
        private SerializedProperty backForce = null;       
        private SerializedProperty usePerHandSettings = null;
        private SerializedProperty leftHandSettings = null;
        private SerializedProperty handSettings = null;
        private SerializedProperty rightHandSettings = null;
        private SerializedProperty unGrabLayer = null;
        private SerializedProperty grabLayer = null;

        private VR_Lever targetScript = null;

        private void OnEnable()
        {
            transformBase = serializedObject.FindProperty( "transformBase" );
            solverIterations = serializedObject.FindProperty( "solverIterations" );
            shouldBackToStartingPosition = serializedObject.FindProperty( "shouldBackToStartingPosition" );
            onValueChange = serializedObject.FindProperty( "onValueChange" );
            backForce = serializedObject.FindProperty( "backForce" );
            onGrabStateChange = serializedObject.FindProperty( "onGrabStateChange" );          
            grabDistance = serializedObject.FindProperty( "interactDistance" );         
            grabButton = serializedObject.FindProperty( "interactButton" );           
            usePerHandSettings = serializedObject.FindProperty( "usePerHandSettings" );
            rightHandSettings = serializedObject.FindProperty( "rightHandSettings" );
            leftHandSettings = serializedObject.FindProperty( "leftHandSettings" );
            handSettings = serializedObject.FindProperty( "handSettings" );
            unGrabLayer = serializedObject.FindProperty("unGrabLayer");
            grabLayer = serializedObject.FindProperty("grabLayer");


            targetScript = (VR_Lever) target;
        }

        public override void OnInspectorGUI()
        {

            PlatinioEditorGUILayout.PropertyField( grabDistance );
            PlatinioEditorGUILayout.PropertyField(usePerHandSettings);

            if (usePerHandSettings.boolValue)
            {
                EditorGUILayout.PropertyField( rightHandSettings, true );
                EditorGUILayout.PropertyField( leftHandSettings, true );
            }
            else
            {
                EditorGUILayout.PropertyField( handSettings, true );
            }

            PlatinioEditorGUILayout.PropertyField( grabButton );
            PlatinioEditorGUILayout.PropertyField(transformBase);
            PlatinioEditorGUILayout.PropertyField(solverIterations);
            PlatinioEditorGUILayout.PropertyField(shouldBackToStartingPosition);
            PlatinioEditorGUILayout.PropertyField(backForce);

            grabLayer.intValue = EditorGUILayout.LayerField( "Grab Layer", grabLayer.intValue );
            unGrabLayer.intValue = EditorGUILayout.LayerField( "UnGrab Layer", unGrabLayer.intValue );

            PlatinioEditorGUILayout.PropertyField(onValueChange);
            PlatinioEditorGUILayout.PropertyField( onGrabStateChange );

           

            //layer = EditorGUILayout.LayerField(layer);
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
            
            EditorGUI.BeginChangeCheck();

            Handles.color = Color.blue;

           
            if (usePerHandSettings.boolValue)
            {
                float rightValue = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointRightHand == null ? targetScript.transform.position : targetScript.HighlightPointRightHand.position, grabDistance.floatValue );
                float leftValue = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointLeftHand == null ? targetScript.transform.position : targetScript.HighlightPointLeftHand.position, grabDistance.floatValue );

                UpdateGrabDistanceValue( rightValue, leftValue );
            }
            else
            {
                if (targetScript.HighlightPointHandSettings == null)
                    return;

                float value = Handles.RadiusHandle( Quaternion.identity, targetScript.HighlightPointHandSettings.position, grabDistance.floatValue );
                targetScript.SetInteractDistanceViaInspector( value );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }

        }

        private void UpdateGrabDistanceValue(float rightValue, float leftValue)
        {
            if (grabDistance.floatValue != rightValue)
            {
                targetScript.SetInteractDistanceViaInspector( rightValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
            else if (grabDistance.floatValue != leftValue)
            {
                targetScript.SetInteractDistanceViaInspector( leftValue );
                EditorUtility.SetDirty( targetScript );
                EditorSceneManager.MarkAllScenesDirty();
            }
        }

    }

}

