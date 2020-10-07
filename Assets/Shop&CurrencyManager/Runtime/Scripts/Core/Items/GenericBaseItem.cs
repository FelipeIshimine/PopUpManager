#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

namespace ShopSystem
{
    public abstract class GenericItem<T> : BaseItem
    {
            public T Value;
    }
}