using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryItem : MonoBehaviour
{
    public Image img;
    public Text AmmountText;
    
    public void Initialize(Sprite icon, int ammount)
    {
        img.sprite = icon;
        UpdateValue(ammount);
    }
    
    public void UpdateValue(int newvalue)
    {
        AmmountText.text = $"x{newvalue}";
    }
}