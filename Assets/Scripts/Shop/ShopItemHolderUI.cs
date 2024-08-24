using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopItemHolderUI : MonoBehaviour
{
    public TextMeshProUGUI Name; 
    public TextMeshProUGUI Cost;
    //public Image Icon;
    public Button Btn;
    public GameObject Prefab;
    //public GameObject Table;

    public void Setup(string name,int cost/*,Sprite icon = null*/,GameObject _prefab)
    {
        Name.text = name;
        Cost.text = cost.ToString();
        //Icon.sprite = icon;
        Prefab = _prefab;
    }
    public void BuyThisProduct()
    {
        //TODO:Buy Currency check 
        ShopManager.Instance.CurrentSelectTable.SetOnTable(Prefab);
        var currentSelectTable= ShopManager.Instance.CurrentSelectTable;
        ShopManager.Instance.CloseShop();
        InputManager.Instance.InteractionButtonClick(currentSelectTable);
        
    }
}
