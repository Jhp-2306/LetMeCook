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

    public void Setup(string name,int cost, string _prefab,EquipmentType _type, Sprite icon = null)
    {
        Name.text = name;
        Cost.text = cost.ToString();
        Icon.sprite = icon;
        PrefabID = _prefab;
        type = _type;
    }
    public void BuyThisProduct()
    {
        //TODO:Buy Currency check 
       
        //close the shop
        ShopManager.Instance.CloseShop();
        //go into building mode
        ShopManager.Instance.BuildModeActivate(PrefabID);
        
        
    }
}
