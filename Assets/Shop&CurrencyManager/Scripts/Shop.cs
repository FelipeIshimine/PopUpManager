using System.Collections.Generic;

namespace ShopSystem
{
    [System.Serializable]
    public class Shop
    {
        public bool canRepeatProdBetweenCatalogs = false;
        
        public List<BaseCatalog> catalogs;
        public List<BaseProduct> allProducts = new List<BaseProduct>();
        public List<BaseProduct> availableProducts = new List<BaseProduct>();
        public List<BaseProduct> unavailableProducts = new List<BaseProduct>();

        public void Initialize()
        {
            allProducts.Clear();
            availableProducts.Clear();
            foreach (BaseCatalog catalog in catalogs)
            {
                allProducts.AddRange(catalog.Products);
                foreach (BaseProduct product in catalog.Products)
                {
                    availableProducts.Add(product);
                }
            }
        }

        public void UpdateWith(Inventory inventory)
        {
            
        }
    }
}