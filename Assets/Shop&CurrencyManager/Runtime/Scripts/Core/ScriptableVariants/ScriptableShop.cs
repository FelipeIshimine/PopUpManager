using System.Collections;
using UnityEditor;
using UnityEngine;

namespace MarketSystem
{
    [CreateAssetMenu(menuName = "Create MainShop", fileName = "MainShop", order = 0)]
    public class ScriptableShop : ScriptableObject
    {
         public Shop value;
    }
}