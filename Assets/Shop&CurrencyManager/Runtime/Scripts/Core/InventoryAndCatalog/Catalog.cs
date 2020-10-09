using System.Collections.Generic;
using UnityEngine;

namespace MarketSystem
{
    public abstract class BaseCatalog : ScriptableObject
    {
        public abstract List<BaseProduct> Products { get; }
    }
     
    public abstract class Catalog<T> : BaseCatalog where T : BaseProduct, new()
    {
        public override List<BaseProduct> Products => new List<BaseProduct>(products);
        public List<T> products;
    }
}