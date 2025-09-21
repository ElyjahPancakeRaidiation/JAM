using System;
using System.IO;
using UnityEngine;

namespace CustomFileFunc
{
    
    public class CustomFuncs
    {

        public static string CombineStringFileName(params string[] names)
        {
            string combinedName = "";
            for (int i = 0; i < names.Length; i++)
            {
                combinedName += Path.AltDirectorySeparatorChar + names[i];
            }
            return combinedName;
        }

        public static string FindFileLocation(string fileName, params string[] extraFileNames)
        {
            return Application.persistentDataPath + CombineStringFileName(extraFileNames) + Path.AltDirectorySeparatorChar + fileName;
        }
    
        public static bool CreateFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                return false;
            }
            Directory.CreateDirectory(folderPath);
            return true;
        }
        public static bool CreateFile(string jsonPath)
        {

            if (File.Exists(jsonPath))
            {
                return false;
            }
            FileStream file = File.Open(jsonPath, FileMode.OpenOrCreate);
            file.Close();
            return true;
        }
    }
}