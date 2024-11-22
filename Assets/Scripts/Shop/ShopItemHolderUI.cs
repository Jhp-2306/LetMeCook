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
    //public Image Icon;
    public Button Btn;
    public GameObject Prefab;
    //public GameObject Table;
    EquipmentType type;

    public void Setup(string name,int cost/*,Sprite icon = null*/,GameObject _prefab,EquipmentType _type)
    {
        Name.text = name;
        Cost.text = cost.ToString();
        //Icon.sprite = icon;
        Prefab = _prefab;
        type = _type;
    }
    public void BuyThisProduct()
    {
        //TODO:Buy Currency check 
        ShopManager.Instance.CurrentSelectTable.SetOnTable(Prefab, Name.text,type);
        var currentSelectTable= ShopManager.Instance.CurrentSelectTable;
        ShopManager.Instance.CloseShop();
        InputManager.Instance.InteractionButtonClick(currentSelectTable);
        
    }
}
