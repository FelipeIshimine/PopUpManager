using System;
using System.Collections.Generic;
using Leguar.TotalJSON;
using SaveSystem;

namespace ShopSystem
{
    [System.Serializable]
    public class Inventory : ISaveLoadAsJson
    {
        public Action<string, int> OnItemChange;
        public Dictionary<string, int> Items = new Dictionary<string, int>();

        public int this[string id]
        {
            get => Get(id);
            set => Set(id, value);
        }

        public int Get(string id) => (Items.ContainsKey(id)) ? Items[id] : 0;

        public void Set(string id, int ammount)
        {
            Items[id] = ammount;
            OnItemChange?.Invoke(id, ammount);
        }

        public void Modify(string id, int ammount) => this[id] += ammount;

        public JSON GetSaveData()
        {
            JSON json = JSON.Serialize(Items);
            return json;
        }

        public void LoadSaveData(JSON json)
        {
            Items = json.Deserialize<Dictionary<string, int>>();
        }
    }

    [System.Serializable]
    public struct ItemData
    {
        public string id;
        public int ammount;
    }
}