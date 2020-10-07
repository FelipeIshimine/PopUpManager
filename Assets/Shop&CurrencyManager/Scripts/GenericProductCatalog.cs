using UnityEngine;

namespace ShopSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Catalog/GenericProduct", fileName = "GenericItemCatalog", order = 0)]
    public class GenericProductCatalog : Catalog<GenericProduct>
    {
    }
}