using UnityEngine;

namespace MarketSystem
{
    [System.Serializable]
    public class ItemRequirement
    {
        public BaseItem item;
        [Min(0)]public int ammount;
        public bool consumes = false;
    }
}