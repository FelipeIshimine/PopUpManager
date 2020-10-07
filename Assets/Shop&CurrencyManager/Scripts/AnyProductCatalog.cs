using UnityEngine;

namespace ShopSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Catalog/AnyProduct", fileName = "AnyItemCatalog", order = 0)]
    public class AnyProductCatalog : Catalog<BaseProduct>
    {
    }
}