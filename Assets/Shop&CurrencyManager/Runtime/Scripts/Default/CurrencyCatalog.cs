using System.Collections;
using System.Collections.Generic;
using ShopSystem;
using UnityEngine;

namespace ShopSystem
{
    [CreateAssetMenu(menuName = "ShopSystem/Catalog/CurrencyCatalog", fileName = "CurrencyCatalog", order = 0)]
    public class CurrencyCatalog : Catalog<Currency.Product>
    {

    }
}