using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using MarketSystem;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MarketSystem
{
    public class MarketManager : RuntimeScriptableSingleton<MarketManager>
    {
        public override MarketManager Myself => this;
        public List<BaseItem> Items = new List<BaseItem>();
        public List<BaseProduct> Products = new List<BaseProduct>();
        public List<BaseCatalog> Catalogs = new List<BaseCatalog>();

        public string folderName = "_MarketSystem";
        
        public Dictionary<string,List<BaseProduct>> GetItemsPerClassFromCatalog(BaseCatalog catalog)
        {
            RemoveNullEmpty(Catalogs);
            Catalogs.Sort(ScriptableObjectComparer);
            Dictionary<string,List<BaseProduct>> products = new Dictionary<string, List<BaseProduct>>();
            foreach (BaseProduct baseProduct in catalog.Products)
            {
                string key = baseProduct.Item.GetType().FullName;
                if (!products.ContainsKey(key))
                    products.Add(key,new List<BaseProduct>());
                products[key].Add(baseProduct);
            }
            return products;
        }
      
        public Dictionary<string,List<BaseItem>> GetItemsByClass()
        {
            RemoveNullEmpty(Items);
            Items.Sort(ScriptableObjectComparer);
            Dictionary<string,List<BaseItem>> items = new Dictionary<string, List<BaseItem>>();
            foreach (BaseItem baseItem in Items)
            {
                string key = baseItem.GetType().FullName;
                if (!items.ContainsKey(key))
                    items.Add(key,new List<BaseItem>());
                items[key].Add(baseItem);
            }
            return items;
        }
        
        public Dictionary<string,List<BaseProduct>> GetProductsByClass()
        {
            RemoveNullEmpty(Products);
            Products.Sort(ScriptableObjectComparer);
            Dictionary<string,List<BaseProduct>> items = new Dictionary<string, List<BaseProduct>>();
            foreach (BaseProduct product in Products)
            {
                string key = product.GetType().ToString();
                if (!items.ContainsKey(key))
                    items.Add(key,new List<BaseProduct>());
                items[key].Add(product);
            }
            return items;
        }

        private static int ScriptableObjectComparer(ScriptableObject x, ScriptableObject y) => string.Compare(x.name, y.name, StringComparison.Ordinal);

        private void RemoveNullEmpty<T>(List<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if(list[i] == null)
                    list.RemoveAt(i);
        }

        public void AddNewItem(BaseItem uObject)
        {
            if(Items.Contains(uObject))
                Debug.LogError("El objeto ya existe");
            else
                Items.Add(uObject);
        }

        public void RemoveItem<T>(T item) where T : BaseItem
        {
            if (Items.Contains(item))
                Items.Remove(item);
        }

        public void AddNewProduct(BaseProduct uObject)
        {
            if(Products.Contains(uObject))
                Debug.LogError("El objeto ya existe");
            else
                Products.Add(uObject);
        }

        public bool DoesItemExists(string assetName) => Items.Find(x => x.name == assetName) != null;

        public bool DoesProductExists(string assetName) => Products.Find(x => x.name == assetName) != null;

        public void RemoveProduct<T>(T product) where T : BaseProduct
        {
            if (Products.Contains(product))
                Products.Remove(product);
        }
    }
}