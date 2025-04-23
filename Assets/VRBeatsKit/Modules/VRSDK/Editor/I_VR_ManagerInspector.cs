using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Platinio.SDK.EditorTools;

namespace VRSDK.EditorCode
{
    [CustomEditor( typeof( VR_Manager ) )]
    public class I_VR_ManagerInspector : Editor
    {
        private SerializedProperty currentSDK = null;        
        private SerializedProperty gestureConfig = null;

        const string DEFINES_FILE_PATH = "Assets/mcs.rsp";
        const string STEAM_VR_DEFINITION = "SDK_STEAM_VR";
        const string OCULUS_VR_DEFINITION = "SDK_OCULUS";
        const string UNITY_XR_DEFINITION = "UNITY_XR";
        const string VR_MANAGER_FILE_PATH = "VRLAB/Scripts/VR/VR_Manager.cs";

        private int startSDKIndex = 0;      
        private static VR_Manager targetScript = null;

        private void Awake()
        {
            currentSDK = serializedObject.FindProperty( "currentSDK" );            
            gestureConfig = serializedObject.FindProperty( "gestureConfig" );            

            targetScript = target as VR_Manager;
            targetScript.SetCurrentSDKViaEditor( GetCurrentEnableSDK() );
            startSDKIndex = (int) targetScript.CurrentSDK;
        }

        private VR_SDK GetCurrentEnableSDK()
        {
            List<string> defList = GlobalDefinitionsManager.GetCurrentDefinitios();


            for (int n = 0; n < defList.Count; n++)
            {
                string def = defList[n].Replace( " ", "" );


                if (def == STEAM_VR_DEFINITION)
                {
                    return VR_SDK.Steam_VR;
                }
                else if (def == OCULUS_VR_DEFINITION)
                {
                    return VR_SDK.Oculus;
                }
                else if (def == UNITY_XR_DEFINITION)
                {
                    return VR_SDK.UnityXR;
                }
            }

            return VR_SDK.None;
        }

        public override void OnInspectorGUI()
        {
            if (currentSDK == null)
                return;

            PlatinioEditorGUILayout.PropertyField( currentSDK );            
            EditorGUILayout.PropertyField( gestureConfig , true );

           

            serializedObject.ApplyModifiedProperties();

            
            if ((int) targetScript.CurrentSDK != startSDKIndex)
            {
                if (GUILayout.Button( "Update SDK" ))
                {
                    UpdateDefinitions(targetScript.CurrentSDK);
                }
            }

#if SDK_STEAM_VR

            if (GUILayout.Button( "Create Input Bindings" ))
            {
                if (EditorUtility.DisplayDialog( "Create Input Bindings", "Are you sure you want to overwrite your current SteamVR Input Bindings", "Overwrite", "Cancel" ))
                {
                    CreateSteamVRInputBindings();
                }
            }
#endif

        }

        private void CreateSteamVRInputBindings()
        {
            Directory.CreateDirectory(Application.dataPath + "/StreamingAssets/SteamVR" );
            CopyFilesFromTo( Application.dataPath + "/VRShooterKit/SteamVR_Default_Input" , Application.dataPath + "/StreamingAssets/SteamVR" );
            AssetDatabase.Refresh();
        }

        private void CopyFilesFromTo(string from , string to)
        {
            if (System.IO.Directory.Exists( from ))
            {
                string[] files = System.IO.Directory.GetFiles( from );

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = Path.GetFileName( s );
                    string destFile = Path.Combine( to, fileName );
                    File.Copy( s, destFile, true );
                }
            }
        }

        private void UpdateDefinitions(VR_SDK targetSDK)
        {
            targetScript.SetCurrentSDKViaEditor( targetSDK );
            startSDKIndex = (int) targetScript.CurrentSDK;

            if (targetSDK == VR_SDK.None)
            {
                GlobalDefinitionsManager.RemoveDefinitions(OCULUS_VR_DEFINITION , STEAM_VR_DEFINITION , UNITY_XR_DEFINITION);
                OnUpdateDefinitions(targetSDK);
                return;
            }

            string defString = GetDefString( targetSDK );


            if (GlobalDefinitionsManager.DefinitionExits( OCULUS_VR_DEFINITION ) || GlobalDefinitionsManager.DefinitionExits( STEAM_VR_DEFINITION ) || GlobalDefinitionsManager.DefinitionExits(UNITY_XR_DEFINITION))
            {
                List<string> defList = GlobalDefinitionsManager.GetCurrentDefinitios();
              
                for (int n = 0; n < defList.Count; n++)
                {
                    string def = defList[n].Replace( " ", "" );

                    //remove all sdk defines
                    if (def == STEAM_VR_DEFINITION || def == OCULUS_VR_DEFINITION || def == UNITY_XR_DEFINITION)
                    {
                        defList.RemoveAt( n );
                        n--;
                    }
                }

                defList.Add( defString );
                GlobalDefinitionsManager.WriteDefinitions( defList );
            }
            else
            {               
                GlobalDefinitionsManager.CreateAndWriteDefinition( defString );
            }

            OnUpdateDefinitions( targetSDK );
        }

        private string GetDefString(VR_SDK sdk)
        {
            if (sdk == VR_SDK.Oculus)
            {
                return OCULUS_VR_DEFINITION;
            }
            else if (sdk == VR_SDK.Steam_VR)
            {
                return STEAM_VR_DEFINITION;
            }
            else if (sdk == VR_SDK.UnityXR)
            {
                return UNITY_XR_DEFINITION;
            }

            return "";
        }

       

        private void OnUpdateDefinitions(VR_SDK targetSDK)
        {
            ForceRebuild();
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset( VR_MANAGER_FILE_PATH, ImportAssetOptions.ForceUpdate );
            AssetDatabase.ImportAsset( DEFINES_FILE_PATH, ImportAssetOptions.ForceUpdate );
            targetScript.SetCurrentSDKViaEditor( targetSDK );
        }

        
        public static void ForceRebuild()
        {
            string[] rebuildSymbols = { "RebuildToggle1", "RebuildToggle2" };
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup );
            var definesStringTemp = definesString;
            if (definesStringTemp.Contains( rebuildSymbols[0] ))
            {
                definesStringTemp = definesStringTemp.Replace( rebuildSymbols[0], rebuildSymbols[1] );
            }
            else if (definesStringTemp.Contains( rebuildSymbols[1] ))
            {
                definesStringTemp = definesStringTemp.Replace( rebuildSymbols[1], rebuildSymbols[0] );
            }
            else
            {
                definesStringTemp += ";" + rebuildSymbols[0];
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                definesStringTemp );
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                definesString );
        }

    }
}

