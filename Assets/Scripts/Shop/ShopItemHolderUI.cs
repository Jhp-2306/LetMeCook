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

    public void Setup(string name,int cost/*,Sprite icon = null*/)
    {
        Name.text = name;
        Cost.text = cost.ToString();
        //Icon.sprite = icon;
    }
    public void BuyThisProduct()
    {
        //TODO:Buy Currency check 
    }
}
