using System.IO;
using NaughtyAttributes;
using MarketSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "ShopSystem/Item/SpriteItem", fileName = "SpriteItem", order = 0)]
public class AudioItem : GenericItem<AudioClip>
{
    [System.Serializable]
    public class Product : Product<AudioItem>
    {
    }
}