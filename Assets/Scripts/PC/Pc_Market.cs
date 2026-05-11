using Constants;
using HandHeld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pc_Market : MonoBehaviour
{
    public struct cartData
    {
        public string name;
        public float price;
        public float quantity;
        public Pc_Market_IngredientSliders slider;
    }
    public GameObject ShopPage, CheckoutPage;
    public List<Pc_Market_IngredientSliders> Sliders;
    private Dictionary<string, cartData> onCart;
    public TMPro.TextMeshProUGUI CartCount;
    public TMPro.TextMeshProUGUI TotalPrice;
    public SO_PCMarketItemList ItemList;
    public List<Pc_MarketItemsDetails> Items;
    public GameObject SliderPrefab;
    public GameObject SliderPrefabParent1, SliderPrefabParent2;
    public GameObject SliderCheckoutPrefab;
    public GameObject CheckoutSliderParent;
    public List<Pc_Checkout_Slider> checkout_Sliders;
    public Vector3 position;
    public void checkout()
    {
        ShopPage.SetActive(false);
        CheckoutPage.SetActive(true);
        SetCheckOut();
    }
    public void MarketTab()
    {
        ShopPage.SetActive(true);
        CheckoutPage.SetActive(false);
    }
    public void Init()
    {
        onCart = new Dictionary<string, cartData>();
        MarketTab();
        Items = ItemList.items;
        var tempbool = true;
        if(Sliders.Count!=Items.Count)
        foreach (var item in Items)
        {
            var go = Instantiate(SliderPrefab, tempbool ? SliderPrefabParent1.transform : SliderPrefabParent2.transform);
            go.GetComponent<Pc_Market_IngredientSliders>().SetUp(item.name, item.des, item.icon_Name, this);
            Sliders.Add(go.GetComponent<Pc_Market_IngredientSliders>());
            tempbool = !tempbool;
        }
    }
    public void AddToCart(string name, cartData data)
    {
        if (onCart.ContainsKey(name))
        {
            onCart[name] = data;
        }
        else
            onCart.Add(name, data);
    }
    public void RemoveFromCart(string name)
    {
        if (onCart.ContainsKey(name))
        {
            onCart.Remove(name);
        }
    }
    Pc_MarketItemsDetails GetItemDetailsFromName(string name)
    {
        foreach (var item in Items)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
        }
        return new Pc_MarketItemsDetails();
    }
    void SetCheckOut()
    {
        //Disable all the active sliders
        var Total = 0;
        foreach(var t in checkout_Sliders)
        {
            t.gameObject.SetActive(false);
        }
        if (CheckoutSliderParent != null)
        {
            if (checkout_Sliders.Count <= 0 || checkout_Sliders == null)
            {
                checkout_Sliders = new List<Pc_Checkout_Slider>();
                foreach (var t in onCart.Values)
                {
                    var go = Instantiate(SliderCheckoutPrefab, CheckoutSliderParent.transform);
                    var price = (t.price * t.quantity);
                    go.GetComponent<Pc_Checkout_Slider>().SetCheckout($"{t.name} x{t.quantity}", price.ToString());
                    checkout_Sliders.Add(go.GetComponent<Pc_Checkout_Slider>());
                    Total+= (int)price;
                    //tempbool = !tempbool;
                }
            }
            else
            {
                if (checkout_Sliders.Count < onCart.Count)
                {
                    var idx = 0;
                    var items=onCart.Values.ToList();
                    foreach (var slider in checkout_Sliders)
                    {
                        var price = (items[idx].price * items[idx].quantity);
                        slider.SetCheckout($"{items[idx].name} x{items[idx].quantity}", price.ToString());
                        Total+= (int)price;
                        idx++;
                    }
                    for(int i= idx; i< items.Count; i++)
                    {
                        var t= items[i];
                        var price = (t.price * t.quantity);
                        var go = Instantiate(SliderCheckoutPrefab, CheckoutSliderParent.transform);
                        go.GetComponent<Pc_Checkout_Slider>().SetCheckout($"{t.name} x{t.quantity}", price.ToString());
                        checkout_Sliders.Add(go.GetComponent<Pc_Checkout_Slider>());
                        Total+= (int)price;
                    }
                }
                else
                {
                    var idx = 0;
                    //var items = onCart.Values.ToList();
                    foreach (var items in onCart.Values)
                    {
                        var price = (items.price * items.quantity);
                        checkout_Sliders[idx].SetCheckout($"{items.name} x{items.quantity}", (items.price * items.quantity).ToString());
                        Total += (int)price;
                        idx++;
                    }
                }
            }
            TotalPrice.text = $"{Total}";
        }
        else
        {
            Debug.Log("Nothing to purchase");
        TotalPrice.text = $"{0}";
        }
    }

    public void PayViaCoins()
    {
        //check currency
        ConfirmPurchase();
    }
    public void PayViaADs(){
        //TODO Ads Implementation here
        ConfirmPurchase();
    }
    private void ConfirmPurchase()
    {
        // get free space for the delivery box to drop 
        // add the items which are requested 
        var DBox=AssetLoader.Instance.GetEquipmetPrefab("Delivery box");
        if (DBox != null) {            
            var box = Instantiate(DBox, position,Quaternion.identity).GetComponent<DeliveryBox>();
            box.init(onCart.Values.Count,90f);
            foreach( var t in onCart.Values)
            {
                RefrigeratorItems item = new RefrigeratorItems(GetItemDetailsFromName(t.name).IngredientType,(int)t.quantity);
                box.AddBoxItem(item);
            }
        }
    }
}

[System.Serializable]
public struct Pc_MarketItemsDetails
{
    public string name;
    public string des;
    public string icon_Name;
    public IngredientType IngredientType;
    //public Sprite icon;
    //public int
}
