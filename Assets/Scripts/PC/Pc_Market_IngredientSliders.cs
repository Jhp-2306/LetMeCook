using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Pc_Market;

public class Pc_Market_IngredientSliders : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI des;
    [SerializeField] private TextMeshProUGUI cartCounter;
    //[SerializeField] private GameObject cartCounter_Parent;

    [SerializeField] private int cartCounter_Count;
    [SerializeField] private GameObject cartController;
    [SerializeField] private GameObject cartController_Plus;
    private Pc_Market Market;
    public void SetUp(string _name, string _price, string _icon, Pc_Market market)
    {
        title.text = _name;
        des.text = _price;
        icon.sprite = AssetLoader.Instance.GetIcons(_icon);
        Market = market;
    }
    public void OnAdd()
    {
        cartCounter_Count++;
        cartData temp = new cartData();
        temp.name = title.text;
        temp.price = Int32.Parse(des.text);
        temp.quantity = cartCounter_Count;
        Market.AddToCart(title.text, temp);
        CartCounterVisuals();
    }
    public void OnSub()
    {
        cartCounter_Count--;
        cartData temp = new cartData();
        temp.name = title.text;
        temp.price = Int32.Parse(des.text);
        temp.quantity = cartCounter_Count;
        Market.AddToCart(title.text, temp);
        if (cartCounter_Count <= 0)
        {
            cartCounter_Count = 0;
            Market.RemoveFromCart(title.text);
        }
        CartCounterVisuals();
    }
    public void CartCounterVisuals()
    {
        cartCounter.text = cartCounter_Count.ToString();
        cartController.SetActive(cartCounter_Count > 0);
        cartController_Plus.SetActive(!(cartCounter_Count > 0));
    }

}
