using UnityEngine;

namespace ShopSystem
{
    [CreateAssetMenu(menuName = "Create PlayerSession", fileName = "PlayerSession", order = 0)]
    public class PlayerSession : ScriptableObject
    {
        public string playerName;
        public Inventory playerInventory;
    }
}