﻿using System;
using System.Collections;
using System.Collections.Generic;
using PopUp;
using ShopSystem;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_MainShop : MonoBehaviour
{
    public Text shopName;
    public RectTransform productsContainer;

    public GameObject uiProductPrefab;

    
    [SerializeField] private ScriptableShop scriptableShop;

    private readonly List<UI_Product> products = new List<UI_Product>();

    public ScriptableInventory playerInventory;
    public Inventory PlayerInventory => playerInventory.value;
    
    [Header("PopUps")]
    public TwoOptionsOneImagePopUp confirmationPopUpPrefab;
    public string baseMessage = "you are about to buy <b>'{0}'</b>x{1}, are you sure?";

    public OneOptionPopUp cantBuyPopUp;
    public string cantBuyMessage = "you dont have enough <b>'{0}'</b>";

    public OneOptionPopUp buySuccessPopUp;
    public string successBuyMessage = "you got <b>'{0}x{1}'</b>";

    public bool showConfirmation = false;
    
    private void Awake()
    {
        shopName.text = scriptableShop.name;
        Initialize(scriptableShop.value);
    }

    public void Initialize(Shop shop)
    {
        if (shop == null) return;
        shop.Initialize();
        for (int i = 0; i < shop.allProducts.Count; i++)
        {
            UI_Product uiProduct = Instantiate(uiProductPrefab, productsContainer).GetComponent<UI_Product>();
            products.Add(uiProduct);
            uiProduct.Initialize(shop.allProducts[i], ProductPressed);
        }
    }

    private void ProductPressed(BaseProduct product)
    {
        if (!PlayerInventory.CanBuy(product))
            ShowCantBuyPopUp(product);
        else
        {
            if (showConfirmation)
                PopUpManager.Show(
                    confirmationPopUpPrefab,
                    new PopUp.TwoOptionsOneImageConfig(
                        string.Format(baseMessage, product.ProductName, product.ammount),
                        product.Icon,
                        new PopUpOption("Buy", () => BuyProduct(product)),
                        new PopUpOption("Cancel", null)
                    ));
            else
                BuyProduct(product);
        }
    }

    private void ShowCantBuyPopUp(BaseProduct product)
    {
        PopUpManager.Show(
            cantBuyPopUp,
            new OneOptionConfig(
                string.Format(cantBuyMessage, product.requirement.item.itemName),
                new PopUpOption(string.Empty, null)));
    }

    private void BuyProduct(BaseProduct product)
    {
        if (PlayerInventory.TryConsume(product.requirement))
            PopUpManager.Show(
                buySuccessPopUp,
                new OneOptionConfig(
                    string.Format(successBuyMessage, product.ProductName, product.ammount),
                    new PopUpOption("OK", ()=> PlayerInventory[product.Item] += product.ammount)));
        else
            ShowCantBuyPopUp(product);
    }
}

