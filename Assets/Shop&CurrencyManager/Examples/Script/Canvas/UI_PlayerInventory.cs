using System;
using System.Collections;
using System.Collections.Generic;
using MarketSystem;
using UnityEngine;

public class UI_PlayerInventory : MonoBehaviour
{
    public ScriptableInventory ScriptableInventory;
    public Inventory Inventory => ScriptableInventory.value;

    public GameObject uiInventoryPrefab;
    
    private Dictionary<BaseItem, UI_InventoryItem> uiInventoryItems;
    
    private void Awake()
    {
        Inventory.OnItemChange += OnItemChange;
        uiInventoryItems = new Dictionary<BaseItem, UI_InventoryItem>();

        foreach (KeyValuePair<BaseItem,int> inventoryItem in Inventory.Items)
            CreateNew(inventoryItem.Key, inventoryItem.Value);
    }

    private void OnItemChange(BaseItem item, int oldvalue, int newvalue)
    {
        if (!uiInventoryItems.ContainsKey(item))
            CreateNew(item,newvalue);
        
        uiInventoryItems[item].UpdateValue(newvalue);
    }

    private void CreateNew(BaseItem item, int newvalue)
    {
        UI_InventoryItem uiInventoryItem = Instantiate(uiInventoryPrefab, transform).GetComponent<UI_InventoryItem>();
        uiInventoryItem.Initialize(item.icon,newvalue);
        uiInventoryItems.Add(item,uiInventoryItem);
    }
}