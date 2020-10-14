using UnityEngine;

namespace MarketSystem
{
    public abstract class BaseProduct : ScriptableObject
    {
        public virtual string DefaultNaming => "{0} - " + GetType().Name;

        [SerializeField]private string productName;
        public string ProductName => string.IsNullOrEmpty(productName) ? (Item?Item.itemName:null) : productName;
        [SerializeField]private Sprite icon;
        public abstract BaseItem Item { get; }
        [Min(1)]public int ammount = 1;
        public ItemRequirement requirement;
        [TextArea(2, 20)] public string description;
        public string ItemId => Item.ID;
        public Sprite Icon => icon ? icon : (Item?Item.icon:null);
    }
    
    [System.Serializable]
    public abstract class GenericProduct<T> : BaseProduct where T : BaseItem
    {
        public override BaseItem Item => item;
        public T item;
    }
}