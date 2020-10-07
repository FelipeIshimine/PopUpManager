using UnityEngine;

namespace ShopSystem
{
    public abstract class Item : ScriptableObject
    {
        public string ID => GetType().FullName;
    }
}