using UnityEngine;

namespace ShopSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Item/Currency", fileName = "Currency", order = 0)]
    public class Currency : BaseItem
    {
        [System.Serializable]
        public class Product : Product<Currency>
        {
        }
    }
}