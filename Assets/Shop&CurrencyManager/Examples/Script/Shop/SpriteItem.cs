using System.IO;
using NaughtyAttributes;
using ShopSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "ShopSystem/Item/SpriteItem", fileName = "SpriteItem", order = 0)]
public class SpriteItem : GenericItem<Sprite>
{
    [System.Serializable]
    public class Product : Product<SpriteItem>
    {
    }
}