using UnityEngine;

namespace MarketSystem
{
    public abstract class BaseItem : ScriptableObject
    {
        public virtual string DefaultNaming => "{0} - " + GetType().Name;
        
        public string ID => name;

        public string itemName;
        public Sprite icon;
    }
}