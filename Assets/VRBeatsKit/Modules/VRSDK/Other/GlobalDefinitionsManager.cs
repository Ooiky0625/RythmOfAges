using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRSDK
{
    public static class GlobalDefinitionsManager
    {
        const string DEFINES_FILE_PATH = "Assets/mcs.rsp";

        public static void CreateAndWriteDefinition(string def)
        {
            List<string> currentDefList = GetCurrentDefinitios();
            currentDefList.Add(def);
            WriteDefinitions(currentDefList);
        }

        public static List<string> GetCurrentDefinitios()
        {
            if (!File.Exists( DEFINES_FILE_PATH ))
                return new List<string>();

            string[] lines = File.ReadAllLines( DEFINES_FILE_PATH );

            for (int n = 0; n < lines.Length; n++)
            {
                if (lines[n].StartsWith( "-define:" ))
                {
                    return lines[n].Replace( "-define:", "" ).Split( ';' ).ToList();
                }
            }

            return new List<string>();
        }

        public static void WriteDefinitions(List<string> defList)
        {
            DeleteDefinitionsFile();

            StringBuilder sb = new StringBuilder();
            sb.Append( "-define:" );
            for (int n = 0; n < defList.Count; n++)
            {
                sb.Append( defList[n] );


                if (n < defList.Count - 1)
                {
                    sb.Append( ";" );
                }

            }

            using (StreamWriter writer = new StreamWriter( DEFINES_FILE_PATH, false ))
            {
                writer.Write( sb.ToString() );
            }
        }

        public static void RemoveDefinitions(params string[] definitionArray)
        {
            List<string> currentDef = GetCurrentDefinitios();

            for (int n = 0; n < currentDef.Count; n++)
            {
                for (int j = 0; j < definitionArray.Length; j++)
                {
                    if (currentDef[n] == definitionArray[j])
                    {
                        currentDef.RemoveAt(n);
                        n--;
                    }
                }
            }

            if (currentDef.Count <= 0)
                DeleteDefinitionsFile();
            else
                WriteDefinitions(currentDef);
        }

        public static bool DefinitionExits(string def)
        {
            List<string> defList = GetCurrentDefinitios();

            for (int n = 0; n < defList.Count; n++)
            {
                string currentDef = defList[n].Replace( " ", "" );

                if (currentDef == def)
                {
                    return true;
                }
            }


            return false;
        }

        private static void DeleteDefinitionsFile()
        {
            //delete definitions file
            if (File.Exists( DEFINES_FILE_PATH ))
                File.Delete( DEFINES_FILE_PATH );
        }
    }
}

