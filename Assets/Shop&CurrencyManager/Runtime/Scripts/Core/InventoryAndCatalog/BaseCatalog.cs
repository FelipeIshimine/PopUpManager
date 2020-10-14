using System.Collections.Generic;
using UnityEngine;

namespace MarketSystem
{
    public class BaseCatalog : ScriptableObject
    {
        public virtual string DefaultNaming => "{0} - " + GetType().Name;
        public virtual List<BaseProduct> Products => products;
        [SerializeField] private List<BaseProduct> products = new List<BaseProduct>();
    }
}