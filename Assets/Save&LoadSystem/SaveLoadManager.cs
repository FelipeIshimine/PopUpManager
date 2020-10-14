using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Leguar.TotalJSON;

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

public static class SaveLoadManagerWithVersion
{
    public const int CurrentVersion = 0;
    public const string VersionKey = "version";

    public static void Save(JSON json, string fileName, int encriptionKey = 0)
    {
        if (!json.ContainsKey(VersionKey))
            throw new Exception("File without version code");

        if (json.GetInt(VersionKey) > CurrentVersion)
            throw new Exception("File version is newer than this one");

        if (json.GetInt(VersionKey) < CurrentVersion)
            UpdateSave(ref json);
        
        Debug.Log(json.CreatePrettyString());

        string rawData = json.CreatePrettyString();
        if (encriptionKey != 0) rawData = SecureHelper.EncryptDecrypt(rawData, encriptionKey);
        
        SaveLoadManager.SaveText(fileName, rawData);
    }

    public static JSON Load(string fileName, int encriptionKey = 0)
    {
        string rawData = SaveLoadManager.LoadText(fileName);
        if (encriptionKey != 0) rawData = SecureHelper.EncryptDecrypt(rawData, encriptionKey);
        JSON data = JSON.ParseString(rawData);  
        
        if (data.GetInt(VersionKey) < CurrentVersion)
            UpdateSave(ref data);
        return data;
    }
    
    private static void UpdateSave(ref JSON json)
    {
        int saveFileVersion = json.GetInt(VersionKey);
        switch (saveFileVersion)
        {
            case 0:
                //Ejecutar conversion de 0 a 1
                break;
            case 1:
                //Ejecutar conversion de 1 a 2
                break;
            default:
                throw new NotImplementedException($"Update from {saveFileVersion} to {saveFileVersion+1} is not implemented");
        }
        if (saveFileVersion != CurrentVersion) UpdateSave(ref json);
    }
}


public static class SecureHelper
{
    public static string Hash(string data)
    {
        byte[] textToBytes = Encoding.UTF8.GetBytes(data);
        SHA256Managed sha256 = new SHA256Managed();
        byte[] hashValue = sha256.ComputeHash(textToBytes);
        return GetHexStringFromHash(hashValue);
    }

    private static string GetHexStringFromHash(byte[] hashValue)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hashValue)
            stringBuilder.Append(b.ToString("x2"));
        return stringBuilder.ToString();
    }

    public static string EncryptDecrypt(string data, int key)
    {
        StringBuilder input = new StringBuilder(data);
        StringBuilder output = new StringBuilder(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            char character = input[i];
            character = (char) (character ^ key);
            output.Append(character);
        }
        return output.ToString();
    }
}