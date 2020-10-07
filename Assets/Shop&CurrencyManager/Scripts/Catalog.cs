using System.Collections.Generic;
using UnityEngine;

namespace ShopSystem
{
    public abstract class BaseCatalog : ScriptableObject
    {
        public abstract List<BaseProduct> Products { get; }
    }
     
    public abstract class Catalog<T> : BaseCatalog where T : BaseProduct
    {
        public override List<BaseProduct> Products => new List<BaseProduct>(products);
        public List<T> products;
    }
}