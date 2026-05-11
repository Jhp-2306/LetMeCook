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
    public Button Btn;
    public string PrefabID;
    //public GameObject Table;
    EquipmentType type;
    private int cost;
    public void Setup(string name,int _cost, string _prefab,EquipmentType _type, Sprite icon = null)
    {
        Name.text = name;
        Cost.text = _cost.ToString();
        cost = _cost;
        Icon.sprite = icon;
        PrefabID = _prefab;
        type = _type;
    }
    public void BuyThisProduct()
    {
        //TODO:Buy Currency check 
        //if (!GameDataDNDL.Instance.DoIHaveEnoughCurrency(cost))
        //{
        //    //Failed to purchase
        //    HUDManagerDNDL.Instance.ShowToastMsg("Not Enough currency");
        //    return;
        //}
        //close the shop
        ShopManager.Instance.CloseShop();
        //go into building mode
        ShopManager.Instance.BuildModeActivate(PrefabID,cost);
        
        
    }
}
