using UnityEngine;

namespace ShopSystem
{
    [System.Serializable]
    public abstract class BaseProduct
    {
        public abstract Item Item { get; }
        public ItemRequirement buyRequirement;
        [TextArea(2, 20)] public string description;
    }
    
    [System.Serializable]
    public class Product<T> : BaseProduct where T : Item
    {
        public override Item Item => item;
        public T item;
    }
    
    [System.Serializable]
    public class CurrencyProduct : Product<Currency>
    {
    }
    
    [System.Serializable]
    public class GenericProduct : Product<Item>
    {
    }
}