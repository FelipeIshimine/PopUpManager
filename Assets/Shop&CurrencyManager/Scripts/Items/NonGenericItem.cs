using Leguar.TotalJSON;
using SaveSystem;
using UnityEngine;

namespace ShopSystem
{
    public abstract class NonGenericItem<T> : Item, ISaveLoadAsJson  where T : ISaveLoadAsJson
    {
        public T Value;

        public JSON GetSaveData()
        {
            JSON rValue = new JSON();
            rValue.Add("value", Value.GetSaveData());
            return rValue;
        }

        public void LoadSaveData(JSON json)
        {
            if (json.ContainsKey("value"))
                Value.LoadSaveData(json.Get("value") as JSON);
            else
                Debug.LogWarning($"No se pudo cargar 'value' en {this}");
        }
    }
}