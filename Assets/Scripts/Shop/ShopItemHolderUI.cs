using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Constants;

public class ShopItemHolderUI : MonoBehaviour
{
    public TextMeshProUGUI Name; 
    public TextMeshProUGUI Cost;
    public Image Icon;
    private string icon;
    public Button Btn;
    public string PrefabID;
    //public GameObject Table;
    EquipmentType type;
    private int cost;
    public void Setup(string name,int _cost, string _prefab,EquipmentType _type, string _icon)
    {
        Name.text = name;
        Cost.text = _cost.ToString();
        cost = _cost;
        icon = _icon;
        PrefabID = _prefab;
        type = _type;
    }
    public void RequestIcons()
    {
        Icon.sprite=AssetLoader.Instance.GetIcons(icon);
        //Debug.Log($"requesting icon for{icon}");
    }
    public void BuyThisProduct()
    {
        //TODO:Buy Currency check--Done
        if (!GameDataDNDL.Instance.DoIHaveEnoughCurrency(cost))
        {
            //Failed to purchase
            HUDManagerDNDL.Instance.ShowToastMsg("Not Enough currency");
            return;
        }
        //close the shop
        ShopManager.Instance.CloseShop();
        //go into building mode
        ShopManager.Instance.BuildModeActivate(PrefabID,cost);
        
        
    }
}
