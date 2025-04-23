using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VRSDK.EditorCode
{
    //this script os a little especial and usefull can help you create dinamyc Enum using scriptable objects
    //it creates a small script containing the VR_Tag enum that gest update every time the scritable object changes
    //this VR_Tag is curretly in  use in the DropZone, so it can know what objects can be dropped

    [CustomEditor( typeof( VR_TagsManager ) )]
    public class I_VR_TagsManager : Editor
    {
        private VR_TagsManager targetScript = null;
        private string[] previusTagList = null;

        private const string FileName = "VR_TagEnum";
        private const string FileExtension = ".cs";

        private void Awake()
        {
            targetScript = (VR_TagsManager) target;

            previusTagList = new string[targetScript.TagList.Count];
            targetScript.TagList.CopyTo( previusTagList );
        }

        public override void OnInspectorGUI()
        {

            if (targetScript.TagList == null)
                targetScript.TagList = new List<string>();

            EditorGUILayout.LabelField( "Tags", EditorStyles.boldLabel );

            for (int n = 0; n < targetScript.TagList.Count; n++)
            {
                targetScript.TagList[n] = EditorGUILayout.TextArea( targetScript.TagList[n] );

                if (GUILayout.Button( "-", GUILayout.Width( 20 ) ))
                    targetScript.TagList.RemoveAt( n );
            }

            if (GUILayout.Button( "+", GUILayout.Width( 20 ) ))
                targetScript.TagList.Add( "" );
        }

        /// <summary>
        /// Find a path for our enum holder script
        /// </summary>
        /// <returns></returns>
        private string GetPath()
        {
            //find is there is a file with the same name already
            string[] files = Directory.GetFiles( Application.dataPath, "*" + FileExtension, SearchOption.AllDirectories );

            for (int n = 0; n < files.Length; n++)
            {

                //there is already a script with that name lest replace it
                if (files[n].Contains( FileName ))
                {
                    //we need to use paths relative to the assets folder
                    string globalPath = files[n].Replace( '\\', '/' );
                    int startIndex = globalPath.IndexOf( "Assets", 0 );
                    string relativePath = globalPath.Substring( startIndex, globalPath.Length - ( startIndex ) );

                    return relativePath;
                }
            }


            //find the VRShooterKit folder
            string[] directoryArray = Directory.GetDirectories( Application.dataPath );

            for (int n = 0; n < directoryArray.Length; n++)
            {
                if (directoryArray[n].Contains( "VRShooterKit" ))
                {
                    string path = directoryArray[n];

                    path = path.Replace( '\\', '/' );

                    return path + "/" + FileName + FileExtension;
                }
            }

            //create the VRShooterKit folder
            return Application.dataPath + "/VRShooterKit/" + FileName + FileExtension;
        }

        /// <summary>
        /// update the enum script
        /// </summary>
        private void UpdateEnumScript()
        {

            string script = "namespace VRShooterKit" +
                                "{ " +
                                    "public enum VR_TagsEnum" +
                                    "{";

            if (targetScript.TagList.Count > 0)
            {
                for (int n = 0; n < targetScript.TagList.Count; n++)
                {
                    script += targetScript.TagList[n];

                    if (n != targetScript.TagList.Count - 1)
                        script += ",";
                }
            }
            else
            {
                script += "Empty";
            }


            script += "}";
            script += "}";

            string path = GetPath();

            File.WriteAllText( path, script );

            //rebuild
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );

            //save
            EditorUtility.SetDirty( targetScript );

        }

        /// <summary>
        /// the content of the list is valid?
        /// </summary>      
        private bool IsValid()
        {
            for (int n = 0; n < targetScript.TagList.Count; n++)
            {
                //find empty
                if (targetScript.TagList[n].Length == 0)
                {
                    Debug.LogWarning( "TagManager found empty Tag reverting changes!" );
                    return false;
                }

                //the tag start with a number
                if (char.IsDigit( targetScript.TagList[n][0] ))
                {
                    Debug.LogWarning( "TagManager found invalid Tag name reverting changes!" );
                    return false;
                }

                //find invalid characters
                for (int j = 0; j < targetScript.TagList[n].Length; j++)
                {

                    if (!IsCharValid(targetScript.TagList[n][j]))
                    {
                        Debug.LogWarning( "TagManager found invalid Tag name reverting changes!" );
                        return false;
                    }
                }
            }

            List<string> duplicateList = targetScript.TagList.GroupBy( x => x )
                        .Where( group => group.Count() > 1 )
                        .Select( group => group.Key ).ToList();

            if (duplicateList.Count > 0)
            {
                Debug.LogWarning( "TagManager found repeat Tag reverting changes!" );
                return false;
            }

            return true;
        }

        private bool IsCharValid(char c)
        {
            c = char.ToLower(c);
            return char.IsDigit(c) || (c >= 'a' && c <= 'z' ) || c == '_';
        }

        /// <summary>
        /// remove white spaces from all names
        /// </summary>
        private void RemoveWhiteSpaces()
        {
            for (int n = 0; n < targetScript.TagList.Count; n++)
            {
                targetScript.TagList[n] = targetScript.TagList[n].Replace( " ", string.Empty );
            }
        }

        private void RevertChanges()
        {
            targetScript.TagList = previusTagList.ToList();
        }

        private bool NeedRebuild()
        {

            if (previusTagList.Length != targetScript.TagList.Count)
            {
                return true;
            }

            for (int n = 0; n < targetScript.TagList.Count; n++)
            {
                if (previusTagList[n] != targetScript.TagList[n])
                    return true;
            }

            return false;

        }

        public void OnDestroy()
        {
            //remove white spaces
            RemoveWhiteSpaces();

            if (!IsValid())
            {
                RevertChanges();
                return;
            }

            if (NeedRebuild())
            {
                UpdateEnumScript();
            }

        }
    }

}

