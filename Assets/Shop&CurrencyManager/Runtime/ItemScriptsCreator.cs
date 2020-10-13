using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MarketSystem
{
    public class ItemScriptsCreator
    {
        private const string TemplateExtension = ".cs.txt";
        private const string TemplateFolder = "Assets/ScriptTemplates/";
        private static string SaveFolder => MarketManager.ClassesFolderPath;

        public static void CreateNewItemType(string itemType, bool createBaseItem)
        {
            MarketManager.InitializeFolders();
            Debug.Log($"Create base item:{createBaseItem}");
            CreateTypeFromTemplates(SaveFolder, itemType, createBaseItem);
        }

        private static void CreateTypeFromTemplates(string saveFolder, string newTypeName, bool createBaseClass)
        {
            string baseClassTemplate = "01-ShopSystem__New Class -New Class";
            string genericClassTemplate = "02-ShopSystem__New Class From Generic-New Item";
            string typePath;

            AssetDatabase.CreateFolder(saveFolder, newTypeName);
            saveFolder += $"/{newTypeName}/";
            
            //Base
            if (createBaseClass)
            {
                typePath = CreateScriptFromTemplate(TemplateFolder, saveFolder, baseClassTemplate);
                Process(typePath, newTypeName);
            }

            //Item
            typePath = CreateScriptFromTemplate(TemplateFolder, saveFolder, genericClassTemplate);
            ProcessGeneric(typePath, newTypeName + "Item", newTypeName,
                new[] {"GenericItem", "Item"});

            //Product
            typePath = CreateScriptFromTemplate(TemplateFolder, saveFolder, genericClassTemplate);
            ProcessGeneric(typePath, newTypeName + "Product", newTypeName + "Item",
                new[] {"GenericProduct", "Product"});

            //Catalogo
            typePath = CreateScriptFromTemplate(TemplateFolder, saveFolder, genericClassTemplate);
            ProcessGeneric(typePath, newTypeName + "Catalog", newTypeName + "Product",
                new[] {"GenericCatalog", "Catalog"});

            AssetDatabase.Refresh();
        }

        private static string CreateScriptFromTemplate(string folderPath, string saveFolder, string fileName)
        {
            string templatePath = folderPath + fileName + TemplateExtension;

            TextAsset original = AssetDatabase.LoadAssetAtPath<TextAsset>(templatePath);
            Debug.Log(original.text);
            if (original == null)
            {
                Debug.LogError("Null");
                return null;
            }

            string endFilePath = Application.dataPath + "/" + saveFolder.Replace("Assets/", string.Empty) + "/" +
                                 fileName + TemplateExtension;
            File.WriteAllText(endFilePath, original.text);
            AssetDatabase.Refresh();

            return endFilePath;
        }

        private static void Process(string path, string scriptName, params string[] otherClasses)
        {
            string fileContent = System.IO.File.ReadAllText(path);
            fileContent = fileContent.Replace("#SCRIPTNAME#", scriptName);

            fileContent = fileContent.Replace("#NOTRIM#", string.Empty);

            for (int i = 0; i < otherClasses.Length; i++)
                fileContent = fileContent.Replace($"#OTHER{i}#", otherClasses[i]);

            string originalFileName = Path.GetFileNameWithoutExtension(path);
            string endFilePath = path.Replace(originalFileName, scriptName);

            Debug.Log($"<<<     >>>     EndFilePath:{endFilePath}");
            System.IO.File.WriteAllText(endFilePath.Replace(".txt", ".cs"), fileContent);
            File.Delete(path);
        }

        private static void ProcessGeneric(string path, string scriptName, string genericClass,
            params string[] otherClasses)
        {
            string fileContent = System.IO.File.ReadAllText(path);
            fileContent = fileContent.Replace("#SCRIPTNAME#", scriptName);

            fileContent = fileContent.Replace("#GENERICNAME#", genericClass);

            fileContent = fileContent.Replace("#NOTRIM#", string.Empty);

            for (int i = 0; i < otherClasses.Length; i++)
                fileContent = fileContent.Replace($"#OTHER{i}#", otherClasses[i]);

            string originalFileName = Path.GetFileNameWithoutExtension(path);
            string endFilePath = path.Replace(originalFileName, scriptName);

            Debug.Log($"Created file:{endFilePath}");
            System.IO.File.WriteAllText(endFilePath.Replace(".txt", ".cs"), fileContent);
            File.Delete(path);
        }
    }
}