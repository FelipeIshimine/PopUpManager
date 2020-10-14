using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UnityEditorUtilities
{
    public static readonly string ProjectPath = Application.dataPath + "/";

    public static T[] FindScriptableObjectsRecursibly<T>(string targetFolder)  where T : ScriptableObject
    {
        List<T> foundObjects = new List<T>(FindScriptableObjects<T>(targetFolder, SearchOption.AllDirectories));
        return foundObjects.ToArray();
    }

    public static T[] FindScriptableObjects<T>(string targetFolder, SearchOption searchOption) where T : ScriptableObject
    {
        string[] files = Directory.GetFiles(ProjectPath + targetFolder, "*.asset", searchOption);
        List<T> found = new List<T>();
        for (int i = 0; i < files.Length; i++)
        {
            string path = "Assets/"  + files[i].Replace(ProjectPath,string.Empty);
            T value = AssetDatabase.LoadAssetAtPath<T>(path);
            if (value != null)
                found.Add(value);
        }
        return found.ToArray();
    }
}   