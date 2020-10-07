using UnityEngine;

namespace ShopSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/ScriptableInventory", fileName = "ScriptableInventory", order = 0)]
    public class ScriptableInventory : ScriptableObject
    {
        public Inventory value;
    }
}