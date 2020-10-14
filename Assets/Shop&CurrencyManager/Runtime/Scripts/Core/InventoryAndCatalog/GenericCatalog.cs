using System.Collections.Generic;

namespace MarketSystem
{
    public abstract class GenericCatalog<T> : BaseCatalog where T : BaseProduct, new()
    {
        public override List<BaseProduct> Products => new List<BaseProduct>(products);
        public List<T> products = new List<T>();
    }
}