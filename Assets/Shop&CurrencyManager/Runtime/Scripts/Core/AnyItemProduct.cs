using UnityEngine;

namespace MarketSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Product", fileName = "BaseProduct", order = 0)]
    public class AnyItemProduct : GenericProduct<BaseItem>
    {
    }
}