using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarketSystem;
using UnityEngine.UI;
using System;

public class UI_Product : MonoBehaviour
{
    public event Action<BaseProduct> OnPressed;
        
    [SerializeReference] public BaseProduct BaseProduct;

    public Text nameText;
    public Image iconImg;
    public Text descriptionText;
    public Image costImg;
    public Text costText;
    
    public void Initialize(BaseProduct baseProduct, Action<BaseProduct> onPressedCallback)
    {
        BaseProduct = baseProduct;

        if(iconImg!=null) iconImg.sprite = baseProduct.Icon;
        if(descriptionText!=null) descriptionText.text = baseProduct.description;
        if (nameText != null) nameText.text = baseProduct.ProductName + (baseProduct.ammount==1?string.Empty:$"x{baseProduct.ammount.ToString()}");
        
        if (baseProduct.requirement.item != null)
        {
            if (costImg != null)
            {
                costImg.sprite = baseProduct.requirement.item.icon;
                costImg.enabled = true;
            }
            if(costText!=null) costText.text = baseProduct.requirement.ammount.ToString();
        }
        else
        {
            if(costImg!=null) costImg.enabled = false;
            if(costText!=null) costText.text = "Free";
        }

        if(onPressedCallback != null)
            OnPressed += onPressedCallback;
    }

    public void Pressed()
    {
        OnPressed?.Invoke(BaseProduct);
    }
}
