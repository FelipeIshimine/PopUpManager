using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using MarketSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MarketSystem
{
    public class MarketManager : RuntimeScriptableSingleton<MarketManager>
    {
        public override MarketManager Myself => this;
        public List<BaseItem> items;
        public List<BaseProduct> products;
        public List<BaseCatalog> catalogs;

        public string mainFolderName = "_MarketSystem";
        public static string MainFolderName => Instance.mainFolderName;
        public static string MainFolderPath => "Assets/" + MainFolderName;

        public string classesFolderName = "Classes";
        public static string ClassesFolderName => Instance.classesFolderName;
        public static string ClassesFolderPath => MainFolderPath + "/" + ClassesFolderName;

        private Dictionary<string,BaseItem> _baseItems;
        public Dictionary<string,BaseItem> ItemsByKey
        {
            get
            {
                if (_baseItems == null) InitializeDictionaryFromList(out _baseItems, items);
                return _baseItems;
            }
        }
        
        private Dictionary<string,BaseProduct> _baseProducts;
        public Dictionary<string,BaseProduct> ProductsByKey
        {
            get
            {
                if (_baseProducts == null) InitializeDictionaryFromList(out _baseProducts, products);
                return _baseProducts;
            }
        }
        
        private Dictionary<string,BaseCatalog> _baseCatalogs;
        public Dictionary<string,BaseCatalog> CatalogsByKey
        {
            get
            {
                if (_baseCatalogs == null) InitializeDictionaryFromList(out _baseCatalogs, catalogs);
                return _baseCatalogs;
            }
        }

        private static void InitializeDictionaryFromList<T>(out Dictionary<string, T> dictionary, IReadOnlyList<T> list) where T : ScriptableObject
        {
            dictionary = new Dictionary<string, T>();
            for (int i = 0; i < list.Count; i++) dictionary.Add(list[i].name, list[i]);
        }

        public Dictionary<string, List<BaseProduct>> GetItemsPerClassFromCatalog(BaseCatalog catalog)
        {
            RemoveNullEmpty(catalogs);
            catalogs.Sort(ScriptableObjectComparer);
            Dictionary<string, List<BaseProduct>> products = new Dictionary<string, List<BaseProduct>>();
            foreach (BaseProduct baseProduct in catalog.Products)
            {
                string key = baseProduct.Item.GetType().FullName;
                if (!products.ContainsKey(key))
                    products.Add(key, new List<BaseProduct>());
                products[key].Add(baseProduct);
            }
            return products;
        }

        public Dictionary<string, List<BaseItem>> GetItemsByClass()
        {
            RemoveNullEmpty(this.items);
            this.items.Sort(ScriptableObjectComparer);
            Dictionary<string, List<BaseItem>> itemsByClass = new Dictionary<string, List<BaseItem>>();
            foreach (BaseItem baseItem in this.items)
            {
                string key = baseItem.GetType().FullName;
                if (!itemsByClass.ContainsKey(key))
                    itemsByClass.Add(key, new List<BaseItem>());
                itemsByClass[key].Add(baseItem);
            }
            return itemsByClass;
        }

        public Dictionary<string, List<BaseProduct>> GetProductsByClass()
        {
            RemoveNullEmpty(products);
            products.Sort(ScriptableObjectComparer);
            Dictionary<string, List<BaseProduct>> productsByClass = new Dictionary<string, List<BaseProduct>>();
            foreach (BaseProduct product in products)
            {
                string key = product.GetType().ToString();
                if (!productsByClass.ContainsKey(key))
                    productsByClass.Add(key, new List<BaseProduct>());
                productsByClass[key].Add(product);
            }
            return productsByClass;
        }

        private static int ScriptableObjectComparer(ScriptableObject x, ScriptableObject y) =>
            string.Compare(x.name, y.name, StringComparison.Ordinal);

        private static void RemoveNullEmpty<T>(List<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == null)
                    list.RemoveAt(i);
        }

        public void AddNewItem(BaseItem uObject)
        {
            if (items.Contains(uObject))
                Debug.LogError("El objeto ya existe");
            else
                items.Add(uObject);
            
            if(!ItemsByKey.ContainsKey(uObject.name))
                ItemsByKey.Add(uObject.name, uObject);
        }

        public void RemoveItem<T>(T item) where T : BaseItem
        {
            if (items.Contains(item))
                items.Remove(item);
        }

        public void AddNewProduct(BaseProduct uObject)
        {
            if (products.Contains(uObject))
                Debug.LogError("El objeto ya existe");
            else
                products.Add(uObject);
            
            if(!ProductsByKey.ContainsKey(uObject.name))
                ProductsByKey.Add(uObject.name, uObject);
        }
        
        public void AddNewCatalog(BaseCatalog nCatalog)
        {
            if (catalogs.Contains(nCatalog))
                Debug.LogError("El objeto ya existe");
            else
                catalogs.Add(nCatalog);
            
            if(!CatalogsByKey.ContainsKey(nCatalog.name))
                CatalogsByKey.Add(nCatalog.name, nCatalog);
        }

        public bool DoesItemIDExists(string assetName) => items.Find(x => x.name == assetName) != null;

        public bool DoesProductIDExists(string assetName) => products.Find(x => x.name == assetName) != null;

        public bool DoesCatalogIDExists(string assetName) => catalogs.Find(x => x.name == assetName) != null;
        
        public void RemoveProduct<T>(T product) where T : BaseProduct
        {
            if (products.Contains(product))
                products.Remove(product);
        }

        public void RemoveCatalog(BaseCatalog catalog)
        {
            if (catalogs.Contains(catalog))
                catalogs.Remove(catalog);
        }

#if UNITY_EDITOR

        public void ScanForAssets()
        {
            items = new List<BaseItem>(UnityEditorUtilities.FindScriptableObjectsRecursibly<BaseItem>(mainFolderName));
            products = new List<BaseProduct>(
                UnityEditorUtilities.FindScriptableObjectsRecursibly<BaseProduct>(mainFolderName));
            catalogs = new List<BaseCatalog>(
                UnityEditorUtilities.FindScriptableObjectsRecursibly<BaseCatalog>(mainFolderName));
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void InitializeFolders()
        {
            if (!AssetDatabase.IsValidFolder(MainFolderPath))
                AssetDatabase.CreateFolder("Assets", MainFolderName);
            
            if (!AssetDatabase.IsValidFolder(ClassesFolderPath))
                AssetDatabase.CreateFolder(MainFolderPath, ClassesFolderName);
            
            AssetDatabase.Refresh();
        }

        public static string CreateFoldersForAsset(string folderName, ScriptableObject nItem)
        {
            if (!AssetDatabase.IsValidFolder(MainFolderPath) || !AssetDatabase.IsValidFolder(ClassesFolderPath))
                InitializeFolders();

            string itemsFolder = MainFolderPath + "/" + folderName;

            if (!AssetDatabase.IsValidFolder(itemsFolder))
                AssetDatabase.CreateFolder(MainFolderPath, folderName);

            string itemTypeFolder = itemsFolder + "/" + nItem.GetType().Name;
            string filePath = itemTypeFolder + "/" + nItem.name;

            if (!AssetDatabase.IsValidFolder(itemTypeFolder))
                AssetDatabase.CreateFolder(itemsFolder, nItem.GetType().Name);
            return filePath;
        }
        #endif


      
    }
}