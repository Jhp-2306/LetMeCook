using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class ShopManager : Singletonref<ShopManager>
{
    public class ShopItems
    {
        string name;
        int price;
        Sprite icon;

        public ShopItems(string name, int price, Sprite icon=null)
        {
            this.name = name;
            this.price = price;
            this.icon = icon;
        }
        public string GetName() => name;
        public int GetPrice() => price;
        public Sprite GetIcon() => icon;
    }
    List<ShopItems> Items;
    private void Start()
    {
        Items = new List<ShopItems>();
        for (int i = 0; i < 20; i++)
        {
            var t = new ShopItems($"item{0}", 100 + i);
            Items.Add(t);
        }
    }
    public List<ShopItems> GetItems() => Items;
    
}
