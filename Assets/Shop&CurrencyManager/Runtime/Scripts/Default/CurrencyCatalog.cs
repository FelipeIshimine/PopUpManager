using System.Collections;
using System.Collections.Generic;
using MarketSystem;
using UnityEngine;

namespace MarketSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Catalog/CurrencyCatalog", fileName = "CurrencyCatalog", order = 0)]
    public class CurrencyCatalog : Catalog<Currency.Product>
    {

    }
}