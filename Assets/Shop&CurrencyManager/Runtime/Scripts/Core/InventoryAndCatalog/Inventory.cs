using System;
using System.Collections.Generic;
using Leguar.TotalJSON;
using SaveSystem;
using UnityEngine;

namespace ShopSystem
{
    [System.Serializable]
    public class Inventory : ISaveLoadAsJson
    {
        public delegate void ItemAmmountChangeEvent(BaseItem item, int oldValue, int newValue);
        public event ItemAmmountChangeEvent OnItemChange;
        public Dictionary<BaseItem, int> Items = new Dictionary<BaseItem, int>();

        public int this[BaseItem id]
        {
            get => Get(id);
            set => Set(id, value);
        }

        public int Get(BaseItem id) => (Items.ContainsKey(id)) ? Items[id] : 0;

        public void Set(BaseItem id, int ammount, bool triggerEvent = true)
        {
            int oldValue = ammount;
            Items[id] = ammount;
            if(triggerEvent) OnItemChange?.Invoke(id, oldValue, ammount);
        }

        public bool CanBuy(BaseProduct product)
        {
            if (product.requirement.item == null) return true;
            if (!Items.ContainsKey(product.requirement.item)) return false;
            return Items[product.requirement.item] >= product.requirement.ammount;
        }
        
        public void Modify(BaseItem id, int ammount) => this[id] += ammount;

        public JSON GetSaveData()
        {
            JSON json = new JSON();
            foreach (KeyValuePair<BaseItem,int> keyValuePair in Items)
                json.Add(keyValuePair.Key.ID, keyValuePair.Value);
            
            return json;
        }

        public void LoadSaveData(JSON json)
        {
            Items = new Dictionary<BaseItem, int>();
            for (int i = 0; i < json.Keys.Length; i++)
            {
                BaseItem item = FindItem();
                if(item != null)
                    Items.Add(item, json.GetInt(json.Keys[i]));
            }
        }

        private BaseItem FindItem()
        {
            throw new NotImplementedException();
        }

        public bool TryConsume(BaseItem id, int ammount)
        {
             //No tiene suficiente
            if (!Items.ContainsKey(id) || Items[id] < ammount) return false;
            Modify(id, -ammount);
            return true;
        }

        public bool TryBuy(BaseProduct product)
        {
            if (product.requirement == null || product.requirement.ammount <= 0)
            { 
                Modify(product.Item, product.ammount);
                return true;
            }

            if (product.requirement.ammount > Items[product.requirement.item]) return false;
            
            Modify(product.requirement.item, -product.requirement.ammount);
            Modify(product.Item, product.ammount);
            return true;
        }
    }
}