using UnityEngine;

namespace MarketSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/ScriptableInventory", fileName = "ScriptableInventory", order = 0)]
    public class ScriptableInventory : ScriptableObject
    {
        public Inventory value;
    }
}