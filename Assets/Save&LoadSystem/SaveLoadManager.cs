using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager
{
    public static bool Exists(string filePath) => File.Exists(filePath);

    private static string GetFilePath(string fileName) => Application.persistentDataPath + fileName;
    
    public static void SaveBinary<T>(string fileName, T data)
    {
        string filePath = GetFilePath(fileName);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (filePath); 
        bf.Serialize(file, data);
        file.Close();
    }
       
    public static T LoadBinary<T>(string fileName)
    {
        T data = default;
        string filePath = GetFilePath(fileName);

        if (!Exists(filePath)) //Archivo no existe
            return data;
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filePath, FileMode.Open);
        data = (T) bf.Deserialize(file);
        file.Close();
        return data;
    }
    
    public static void SaveText(string fileName, string data)
    {
        string filePath = GetFilePath(fileName);
        File.WriteAllText(filePath, data);
    }
    
    public static string LoadText(string fileName)
    {
        string filePath = GetFilePath(fileName);
        return !Exists(filePath) ? null : File.ReadAllText(filePath);
    }
}
