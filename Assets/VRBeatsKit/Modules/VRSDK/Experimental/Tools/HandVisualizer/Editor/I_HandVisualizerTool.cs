using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


namespace VRSDK.EditorCode
{
    [CustomEditor(typeof(HandVisualizerTool))]
    public class I_HandVisualizerTool : Editor
    {
        private static VR_Controller activeController = null;
        private VR_Grabbable grabbableClone = null;
        private VR_Grabbable grabbable = null;

        private HandVisualizerTool targetScript = null;

        private static string returnScenePath = null;
        private static Scene returnScene;
        private static bool inPreviewMode = false;

        private void Awake()
        {
            /*
            targetScript = (HandVisualizerTool)target;

            if (activeController == null || grabbableClone == null)
            {
                grabbable = targetScript.GetComponent<VR_Grabbable>();
                grabbableClone = Instantiate( grabbable, grabbable.transform.position, grabbable.transform.rotation );
                DestroyImmediate(grabbableClone.GetComponent<HandVisualizerTool>());
                activeController = Instantiate( VR_Manager.instance.RightController, grabbable.RightInteractPoint.position, Quaternion.identity );

                //try this
                //EditorSceneManager.NewPreviewScene();
                EditorSceneManager.MoveGameObjectToScene()
            }
            */
        }

        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!HandPreviewManager.IsPreviewModeEnable)
            {
                if (GUILayout.Button( "Enter Preview Mode Right Hand" ))
                {
                    EnterPreviewMode(VR_Manager.instance.Player.RightController);
                }

                if (GUILayout.Button( "Enter Preview Mode Left Hand" ))
                {
                    EnterPreviewMode( VR_Manager.instance.Player.LeftController );
                }
            }
            else
            {
                if (GUILayout.Button( "Save and Exit Preview Mode" ))
                {
                    HandPreviewManager.SaveAndExit();
                }

                if (GUILayout.Button( "Exit Preview Mode" ))
                {
                    HandPreviewManager.ExitPreviewMode();
                }
            }
            
        }

        private void EnterPreviewMode(VR_Controller controller)
        {
            targetScript = (HandVisualizerTool) target;

            grabbable = targetScript.GetComponent<VR_Grabbable>();
            grabbable.gameObject.AddComponent<GameObjectMarker>();
            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();

            grabbableClone = Instantiate( grabbable , grabbable.transform.position, grabbable.transform.rotation );            
            activeController = Instantiate( controller , grabbable.RightInteractPoint.position , Quaternion.identity );
            
            grabbableClone.SetEditorGrabPositionAndRotation( activeController );

            

            
            HandPreviewManager.EnterPreviewMode(grabbableClone.transform.root.gameObject);
        }               


    }

}

