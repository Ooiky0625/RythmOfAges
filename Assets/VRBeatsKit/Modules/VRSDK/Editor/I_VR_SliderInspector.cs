using Platinio.SDK.EditorTools;
using UnityEngine;
using UnityEditor;

namespace VRSDK.EditorCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VR_Slider))]
    public class I_VR_SliderInspector : I_VR_GrabbableInspector
    {       
        private SerializedProperty slideAxis = null;      
        private SerializedProperty slideStartMarker = null;
        private SerializedProperty slideEndMarker = null;
        private SerializedProperty onValueChange = null;
        

        protected override void OnEnable()
        {
            slideAxis = serializedObject.FindProperty("slideAxis");
            slideStartMarker = serializedObject.FindProperty("slideStartMarker");
            slideEndMarker = serializedObject.FindProperty("slideEndMarker");
            onValueChange = serializedObject.FindProperty("onValueChange");
            
            base.OnEnable();
        }


        protected override void DrawOtherSettingsBody()
        {
            GUIContent content = new GUIContent("Slide Axis", "The axis use by this object in local space in order to slide");
            slideAxis.enumValueIndex = (int)(Axis)EditorGUILayout.EnumPopup(content, (Axis)slideAxis.enumValueIndex);
            
            content = new GUIContent("Slider Start Marker", "");
            slideStartMarker.objectReferenceValue = EditorGUILayout.ObjectField(content, slideStartMarker.objectReferenceValue, typeof(Transform), true);
            
            content = new GUIContent("Slider End Marker", "");
            slideEndMarker.objectReferenceValue = EditorGUILayout.ObjectField(content, slideEndMarker.objectReferenceValue, typeof(Transform), true);
            
           
            PlatinioEditorGUILayout.PropertyField(onValueChange);
            
            base.DrawOtherSettingsBody();
        }


    }
}

