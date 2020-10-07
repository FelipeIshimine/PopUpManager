using UnityEngine;

namespace ShopSystem
{
    public abstract class BaseItem : ScriptableObject
    {
        public string ID => GetType().FullName + name;

        public string itemName;
        public Sprite icon;
    }
}