using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace VRSDK.EditorCode
{
    public static class HandPreviewManager
    {
#if UNITY_EDITOR
        private static string originalScenePath = null;
        private static bool previewModeEnable = false;
        private static VR_Grabbable inspectedGrabbable = null;
        private static VR_Controller activeController = null;
        private static HandPreviewSave handPreviewSave = null;
       

        public static bool IsPreviewModeEnable { get { return previewModeEnable; } }

        public static void EnterPreviewMode(GameObject clone)
        {
            
            Scene originalScene = EditorSceneManager.GetActiveScene();
            originalScenePath = EditorApplication.currentScene;

            Scene previewScene = EditorSceneManager.NewScene( NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive );
            EditorSceneManager.MoveGameObjectToScene( clone, previewScene );

            EditorSceneManager.UnloadSceneAsync( originalScene );
            previewModeEnable = true;
            inspectedGrabbable = GameObject.FindObjectOfType<VR_Grabbable>();
            activeController = GameObject.FindObjectOfType<VR_Controller>();

            OverrideGrabAnimation();

            activeController.Animator.SetBool("IsGrabbing" , true);

            EditorApplication.update += Update;

        }

        public static void ExitPreviewMode()
        {
            previewModeEnable = false;
            EditorSceneManager.OpenScene(originalScenePath);
            EditorApplication.update -= Update;
        }

        public static void SaveAndExit()
        {
            handPreviewSave = new HandPreviewSave( inspectedGrabbable );
            ExitPreviewMode();
            GameObjectMarker marker = GameObject.FindObjectOfType<GameObjectMarker>();
            handPreviewSave.LoadInto(marker.GetComponent<VR_Grabbable>());
            GameObject.DestroyImmediate(marker);
        }

        private static void Update()
        {
            activeController.Animator.Update(1.0f/30.0f);
            inspectedGrabbable.SetEditorGrabPositionAndRotation(activeController);

            OverrideGrabAnimation();            
        }

        private static void OverrideGrabAnimation()
        {
            VR_HandInteractSettings settings = inspectedGrabbable.GetHandInteractionSettings( activeController );
            //AnimationClip clip = settings.animation;

            //activeController.OverrideInteractAnimation( settings.animation );
        }
    }

    public class HandPreviewSave
    {
        public VR_HandInteractSettings handSettings = null;
        public VR_HandInteractSettings leftSettings = null;
        public VR_HandInteractSettings rightSettings = null;
        public Vector3 handSettingsInteractPointLocalPosition = Vector3.zero;
        public Vector3 rightHandSettingsInteractPointLocalPosition = Vector3.zero;
        public Vector3 leftHandSettingsInteractPointLocalPosition = Vector3.zero;

        public HandPreviewSave(VR_Grabbable grabbable)
        {
            handSettings = grabbable.HandSettings;
            leftSettings = grabbable.LeftHandSettings;
            rightSettings = grabbable.RightHandSettings;

            handSettingsInteractPointLocalPosition = handSettings.interactPoint.localPosition;
            leftHandSettingsInteractPointLocalPosition = leftSettings.interactPoint.localPosition;
            rightHandSettingsInteractPointLocalPosition = rightSettings.interactPoint.localPosition;
        }

        public void LoadInto(VR_Grabbable grabbable)
        {
            CopyTo(handSettings , grabbable.HandSettings);
            CopyTo(rightSettings , grabbable.RightHandSettings);
            CopyTo(leftSettings , grabbable.LeftHandSettings);

            if (grabbable.HandSettings.interactPoint != null)
            {
                Debug.Log( grabbable.HandSettings.interactPoint.localPosition );
                Debug.Log( handSettingsInteractPointLocalPosition );
                grabbable.HandSettings.interactPoint.localPosition = handSettingsInteractPointLocalPosition;
            }
                

            if (grabbable.LeftHandSettings.interactPoint != null)
                grabbable.LeftHandSettings.interactPoint.localPosition = leftHandSettingsInteractPointLocalPosition;

            if (grabbable.RightHandSettings.interactPoint != null)
                grabbable.RightHandSettings.interactPoint.localPosition = rightHandSettingsInteractPointLocalPosition;

        }

        private void CopyTo(VR_HandInteractSettings from , VR_HandInteractSettings to)
        {
            //to.animation = from.animation;
            to.canInteract = from.canInteract;
            to.rotationOffset = from.rotationOffset;            
        }

#endif
    }


}

