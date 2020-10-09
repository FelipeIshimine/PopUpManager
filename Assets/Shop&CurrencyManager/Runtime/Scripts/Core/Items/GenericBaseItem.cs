#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

namespace MarketSystem
{
    public abstract class GenericItem<T> : BaseItem
    {
            public T Value;
    }
}