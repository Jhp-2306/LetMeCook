using Constants;
using HandHeld;
using System.Collections;
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
    public GameObject SliderPrefabParent;
    public GameObject SliderCheckoutPrefab;
    public GameObject CheckoutSliderParent;
    public List<Pc_Checkout_Slider> checkout_Sliders;
    public Vector3 position;
    [SerializeField] private GameObject FTUT_CartBtn;
    [SerializeField] private GameObject FTUT_BuyBtn;
    //Scalablity 
    public GridLayout gridvis;
    private bool isRewardedAd;
    int totalCost = 0;
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
        if (GameDataDNDL.Instance.isFTUT)
        {
            HUDManagerDNDL.Instance.TutorialHUDDisableFocus();
            //HUDManagerDNDL.Instance.TutorialHUDDisableFocus();
            HUDManagerDNDL.Instance.SetTutorialHUD("Add 1xCarrot, 1xTomato, 1xOnion", () =>
            {
                // Start Wait for All ingredient to get selected
                StartCoroutine(FTUT_WaitTillAllIngreident());
            }, true);

        }
        onCart = new Dictionary<string, cartData>();
        CartCount.text = $"{onCart.Keys.Count}";
        MarketTab();
        Items = ItemList.items;
        //gridvis.
        var tempbool = true;
        if (Sliders.Count != Items.Count)
            foreach (var item in Items)
            {
                var go = Instantiate(SliderPrefab, /*tempbool ? */SliderPrefabParent.transform/*1.transform : SliderPrefabParent2.transform*/);
                var isDisable = GameDataDNDL.Instance.isFTUT ?
                    !(item.IngredientType == IngredientType.Tomato || item.IngredientType == IngredientType.Onion || item.IngredientType == IngredientType.Carrot)
                    : false;
                go.GetComponent<Pc_Market_IngredientSliders>().SetUp(item.name, item.des, item.icon_Name, this, isDisable);
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
        {
            onCart.Add(name, data);
            CartCount.text = $"{onCart.Keys.Count}";
        }
    }
    public void RemoveFromCart(string name)
    {
        if (onCart.ContainsKey(name))
        {
            onCart.Remove(name);
            CartCount.text = $"{onCart.Keys.Count}";
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
        
        foreach (var t in checkout_Sliders)
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
                    totalCost += (int)price;
                    //tempbool = !tempbool;
                }
            }
            else
            {
                if (checkout_Sliders.Count < onCart.Count)
                {
                    var idx = 0;
                    var items = onCart.Values.ToList();
                    foreach (var slider in checkout_Sliders)
                    {
                        var price = (items[idx].price * items[idx].quantity);
                        slider.SetCheckout($"{items[idx].name} x{items[idx].quantity}", price.ToString());
                        totalCost += (int)price;
                        idx++;
                    }
                    for (int i = idx; i < items.Count; i++)
                    {
                        var t = items[i];
                        var price = (t.price * t.quantity);
                        var go = Instantiate(SliderCheckoutPrefab, CheckoutSliderParent.transform);
                        go.GetComponent<Pc_Checkout_Slider>().SetCheckout($"{t.name} x{t.quantity}", price.ToString());
                        checkout_Sliders.Add(go.GetComponent<Pc_Checkout_Slider>());
                        totalCost += (int)price;
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
                        totalCost += (int)price;
                        idx++;
                    }
                }
            }
            TotalPrice.text = $"{totalCost}";
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
#if UNITY_EDITOR//Testing
        ConfirmPurchase();
        return;
#endif

        if (GameDataDNDL.Instance.tryDeductingCurrency(totalCost))
        ConfirmPurchase();
        else
        {
            failedPurchase();
        }
    }
    public void PayViaADs()
    {
        //TODO Ads Implementation here
        //disable button
        AdManagerDNDL.Instance.ShowAds(ConfirmPurchase, failedPurchase);
        //ConfirmPurchase();
    }
    private void ConfirmPurchase()
    {
        // get free space for the delivery box to drop
        HUDManagerDNDL.Instance.CloseApplication();
        HUDManagerDNDL.Instance.PcPowerOff();//Close the Pc 
        // add the items which are requested 
        var DBox = AssetLoader.Instance.GetEquipmetPrefab("Delivery box");
        if (DBox != null)
        {
            var box = Instantiate(DBox, position, Quaternion.identity).GetComponent<DeliveryBox>();
            var duration = GameDataDNDL.Instance.isFTUT ? 10f : 90f;
            if (GameDataDNDL.Instance.isFTUT)
                InputManager.Instance._cameraControle.SetTarget(box.gameObject, () =>
            {
                InputManager.Instance._cameraControle.SetTargetBackToPlayer();
            },new Vector3(0,0,2f));
            box.init(onCart.Values.Count, duration);
            foreach (var t in onCart.Values)
            {
                RefrigeratorItems item = new RefrigeratorItems(GetItemDetailsFromName(t.name).IngredientType, (int)t.quantity);
                GameDataDNDL.Instance.UnlockedIngredients(item.Type);
                box.AddBoxItem(item);
            }
            ProficiencySystem.Instance.RefreshProficiency();
            //Reset the slider and all
            foreach (var t in Sliders)
                t.ResetSlider();
            onCart = new Dictionary<string, cartData>();
            
        }
    }

    private void failedPurchase()
    {
        //Show failed ad popup
    }
    bool isAllingreidentSelected()
    {
        return onCart.Values.Count == 3;
    }
    IEnumerator FTUT_WaitTillAllIngreident()
    {
        yield return new WaitUntil(isAllingreidentSelected);
        HUDManagerDNDL.Instance.SetTutorialHUD("Good Job! Let's go to Check Out Page", () =>
        {
            HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_CartBtn.transform, () =>
            {
                checkout();
                HUDManagerDNDL.Instance.SetTutorialHUD(new List<string>(){"there are two option of payment",
                "Pay Via Ads- \n<size=35>this wont consume your coins instead it will let you watch an ads.</size>\n<color=white><size=20>(Basically Free Stuff -Dev Advice O7)</size></color>\n<size=20>**Note: you can claim only 10 items per ad**</size>",
                "Pay Via Coins-\n<size=35>this will consume your coins</size>","Now pay using your coins"}, () =>
                {
                    HUDManagerDNDL.Instance.SetTutorialHUD(FTUT_BuyBtn.transform, () =>
                    {
                        PayViaCoins();
                    }, true, true);
                });


            }, false, true);

        });
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
