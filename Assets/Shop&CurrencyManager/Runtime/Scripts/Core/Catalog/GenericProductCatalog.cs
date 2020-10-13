using UnityEngine;

namespace MarketSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Catalog/GenericProduct", fileName = "GenericItemCatalog", order = 0)]
    public class GenericProductCatalog : GenericCatalog<AnyItemProduct>
    {
    }
}